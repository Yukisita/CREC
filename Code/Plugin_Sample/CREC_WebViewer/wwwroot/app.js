/*
CREC Web Viewer - Frontend Application
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

// Global variables
let currentPage = 1;
let currentPageSize = 20;
let currentSearchCriteria = {};
let currentLanguage = 'ja'; // 'ja' for Japanese, 'en' for English

// Language translations
const translations = {
    ja: {
        'loading': '読み込み中...',
        'search-results': '検索結果',
        'items-found': '件見つかりました',
        'no-results': '検索結果がありません',
        'error-loading': 'データの読み込みでエラーが発生しました',
        'collection-name': 'コレクション名',
        'collection-id': 'ID',
        'category': 'カテゴリ',
        'registration-date': '登録日',
        'management-code': '管理コード',
        'location': '場所',
        'inventory': '在庫数',
        'inventory-status': '在庫状況',
        'tags': 'タグ',
        'comment': 'コメント',
        'images': '画像',
        'files': 'ファイル',
        'no-thumbnail': 'サムネイルなし',
        'view-details': '詳細表示',
        'stock-out': '在庫切れ',
        'under-stocked': '在庫不足',
        'appropriate': '在庫適正',
        'over-stocked': '在庫過剰',
        'not-set': '未設定',
        'page': 'ページ',
        'of': '/',
        'previous': '前へ',
        'next': '次へ'
    },
    en: {
        'loading': 'Loading...',
        'search-results': 'Search Results',
        'items-found': 'items found',
        'no-results': 'No results found',
        'error-loading': 'Error loading data',
        'collection-name': 'Collection Name',
        'collection-id': 'ID',
        'category': 'Category',
        'registration-date': 'Registration Date',
        'management-code': 'Management Code',
        'location': 'Location',
        'inventory': 'Inventory',
        'inventory-status': 'Inventory Status',
        'tags': 'Tags',
        'comment': 'Comment',
        'images': 'Images',
        'files': 'Files',
        'no-thumbnail': 'No thumbnail',
        'view-details': 'View Details',
        'stock-out': 'Stock Out',
        'under-stocked': 'Under Stocked',
        'appropriate': 'Appropriate',
        'over-stocked': 'Over Stocked',
        'not-set': 'Not Set',
        'page': 'Page',
        'of': 'of',
        'previous': 'Previous',
        'next': 'Next'
    }
};

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});

async function initializeApp() {
    await loadFilters();
    await searchCollections();
}

// Load filter options
async function loadFilters() {
    try {
        // Load categories
        const categoriesResponse = await fetch('/api/collections/categories');
        const categories = await categoriesResponse.json();
        populateSelect('categoryFilter', categories);

        // Load tags
        const tagsResponse = await fetch('/api/collections/tags');
        const tags = await tagsResponse.json();
        populateSelect('tag1Filter', tags);
        populateSelect('tag2Filter', tags);
        populateSelect('tag3Filter', tags);
    } catch (error) {
        console.error('Error loading filters:', error);
    }
}

function populateSelect(selectId, options) {
    const select = document.getElementById(selectId);
    const defaultOption = select.querySelector('option[value=""]');
    
    // Clear existing options except the default
    while (select.children.length > 1) {
        select.removeChild(select.lastChild);
    }
    
    options.forEach(option => {
        const optionElement = document.createElement('option');
        optionElement.value = option;
        optionElement.textContent = option;
        select.appendChild(optionElement);
    });
}

// Search collections
async function searchCollections(page = 1) {
    currentPage = page;
    currentPageSize = parseInt(document.getElementById('pageSize').value);
    
    const criteria = {
        searchText: document.getElementById('searchText').value,
        category: document.getElementById('categoryFilter').value,
        tag1: document.getElementById('tag1Filter').value,
        tag2: document.getElementById('tag2Filter').value,
        tag3: document.getElementById('tag3Filter').value,
        inventoryStatus: document.getElementById('inventoryStatusFilter').value || null,
        page: currentPage,
        pageSize: currentPageSize
    };

    currentSearchCriteria = criteria;

    showLoading(true);
    hideError();

    try {
        const queryParams = new URLSearchParams();
        Object.keys(criteria).forEach(key => {
            if (criteria[key] !== null && criteria[key] !== '') {
                queryParams.append(key, criteria[key]);
            }
        });

        const response = await fetch(`/api/collections/search?${queryParams}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        displaySearchResults(result);
        updatePagination(result);
    } catch (error) {
        console.error('Error searching collections:', error);
        showError(t('error-loading') + ': ' + error.message);
    } finally {
        showLoading(false);
    }
}

function displaySearchResults(result) {
    const grid = document.getElementById('collectionsGrid');
    const summary = document.getElementById('resultsSummary');
    const resultsText = document.getElementById('resultsText');
    const resultsCount = document.getElementById('resultsCount');

    // Clear previous results
    grid.innerHTML = '';

    if (result.collections.length === 0) {
        grid.innerHTML = `
            <div class="col-12">
                <div class="text-center py-5">
                    <i class="bi bi-search display-1 text-muted"></i>
                    <h4 class="mt-3 text-muted">${t('no-results')}</h4>
                </div>
            </div>
        `;
        summary.style.display = 'none';
        return;
    }

    // Update summary
    resultsText.textContent = `${t('search-results')}: ${result.totalCount} ${t('items-found')}`;
    resultsCount.textContent = result.totalCount;
    summary.style.display = 'block';

    // Display collections
    result.collections.forEach(collection => {
        const card = createCollectionCard(collection);
        grid.appendChild(card);
    });
}

function createCollectionCard(collection) {
    const colDiv = document.createElement('div');
    colDiv.className = 'col-lg-3 col-md-4 col-sm-6';

    const inventoryStatusText = getInventoryStatusText(collection.collectionInventoryStatus);
    const inventoryBadgeClass = getInventoryStatusBadgeClass(collection.collectionInventoryStatus);

    const tagsHtml = [
        collection.collectionTag1,
        collection.collectionTag2,
        collection.collectionTag3
    ].filter(tag => tag && tag !== ' - ')
     .map(tag => `<span class="badge bg-secondary tag-badge">${escapeHtml(tag)}</span>`)
     .join('');

    const thumbnailHtml = collection.thumbnailPath 
        ? `<img src="/api/files/${encodeURIComponent(collection.collectionID)}/${encodeURIComponent(collection.thumbnailPath)}" class="card-img-top" alt="Thumbnail">`
        : `<div class="thumbnail-placeholder">
               <i class="bi bi-image display-4"></i>
               <br><small>${t('no-thumbnail')}</small>
           </div>`;

    colDiv.innerHTML = `
        <div class="card h-100 collection-card">
            ${thumbnailHtml}
            <div class="card-body">
                <h6 class="card-title">${escapeHtml(collection.collectionName)}</h6>
                <p class="card-text">
                    <small class="text-muted">ID: ${escapeHtml(collection.collectionID)}</small><br>
                    <small class="text-muted">${t('category')}: ${escapeHtml(collection.collectionCategory)}</small><br>
                    ${collection.collectionCurrentInventory !== null ? 
                        `<small class="text-muted">${t('inventory')}: ${collection.collectionCurrentInventory}</small><br>` : ''}
                    <span class="badge ${inventoryBadgeClass}">${inventoryStatusText}</span>
                </p>
                ${tagsHtml}
            </div>
            <div class="card-footer">
                <button class="btn btn-primary btn-sm w-100" onclick="showCollectionDetails('${escapeHtml(collection.collectionID)}')">
                    <i class="bi bi-eye"></i> ${t('view-details')}
                </button>
            </div>
        </div>
    `;

    return colDiv;
}

function getInventoryStatusText(status) {
    const statusMap = {
        0: t('stock-out'),
        1: t('under-stocked'),
        2: t('appropriate'),
        3: t('over-stocked'),
        4: t('not-set')
    };
    return statusMap[status] || t('not-set');
}

function getInventoryStatusBadgeClass(status) {
    const classMap = {
        0: 'bg-danger',
        1: 'bg-warning',
        2: 'bg-success',
        3: 'bg-info',
        4: 'bg-secondary'
    };
    return classMap[status] || 'bg-secondary';
}

async function showCollectionDetails(collectionId) {
    try {
        const response = await fetch(`/api/collections/${encodeURIComponent(collectionId)}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const collection = await response.json();
        displayCollectionModal(collection);
    } catch (error) {
        console.error('Error loading collection details:', error);
        alert(t('error-loading') + ': ' + error.message);
    }
}

function displayCollectionModal(collection) {
    const modal = new bootstrap.Modal(document.getElementById('collectionModal'));
    const modalTitle = document.getElementById('modalTitle');
    const modalBody = document.getElementById('modalBody');

    modalTitle.textContent = collection.collectionName;

    const inventoryStatusText = getInventoryStatusText(collection.collectionInventoryStatus);
    const inventoryBadgeClass = getInventoryStatusBadgeClass(collection.collectionInventoryStatus);

    const imagesHtml = collection.imageFiles.length > 0 
        ? collection.imageFiles.map(img => `
            <div class="col-md-6 mb-3">
                <img src="/api/files/${encodeURIComponent(collection.collectionID)}/${encodeURIComponent(img)}" 
                     class="img-fluid rounded" alt="${escapeHtml(img)}" style="max-height: 300px;">
                <p class="small text-muted mt-1">${escapeHtml(img)}</p>
            </div>
          `).join('')
        : `<p class="text-muted">${t('no-images')}</p>`;

    const filesHtml = collection.otherFiles.length > 0
        ? collection.otherFiles.map(file => `
            <li class="list-group-item d-flex justify-content-between align-items-center">
                ${escapeHtml(file)}
                <a href="/api/files/${encodeURIComponent(collection.collectionID)}/${encodeURIComponent(file)}" 
                   class="btn btn-sm btn-outline-primary" target="_blank">
                    <i class="bi bi-download"></i>
                </a>
            </li>
          `).join('')
        : `<p class="text-muted">No files</p>`;

    modalBody.innerHTML = `
        <div class="row">
            <div class="col-md-6">
                <h6>${t('collection-id')}</h6>
                <p>${escapeHtml(collection.collectionID)}</p>
                
                <h6>${t('management-code')}</h6>
                <p>${escapeHtml(collection.collectionMC)}</p>
                
                <h6>${t('category')}</h6>
                <p>${escapeHtml(collection.collectionCategory)}</p>
                
                <h6>${t('registration-date')}</h6>
                <p>${escapeHtml(collection.collectionRegistrationDate)}</p>
            </div>
            <div class="col-md-6">
                <h6>${t('location')}</h6>
                <p>${escapeHtml(collection.collectionRealLocation)}</p>
                
                <h6>${t('inventory')}</h6>
                <p>${collection.collectionCurrentInventory !== null ? collection.collectionCurrentInventory : t('not-set')}</p>
                
                <h6>${t('inventory-status')}</h6>
                <p><span class="badge ${inventoryBadgeClass}">${inventoryStatusText}</span></p>
                
                <h6>${t('tags')}</h6>
                <p>
                    ${[collection.collectionTag1, collection.collectionTag2, collection.collectionTag3]
                        .filter(tag => tag && tag !== ' - ')
                        .map(tag => `<span class="badge bg-secondary me-1">${escapeHtml(tag)}</span>`)
                        .join('') || t('not-set')}
                </p>
            </div>
        </div>
        
        ${collection.comment ? `
            <div class="row mt-3">
                <div class="col-12">
                    <h6>${t('comment')}</h6>
                    <div class="border rounded p-3" style="white-space: pre-wrap;">${escapeHtml(collection.comment)}</div>
                </div>
            </div>
        ` : ''}
        
        <div class="row mt-3">
            <div class="col-12">
                <h6>${t('images')}</h6>
                <div class="row">
                    ${imagesHtml}
                </div>
            </div>
        </div>
        
        <div class="row mt-3">
            <div class="col-12">
                <h6>${t('files')}</h6>
                <ul class="list-group">
                    ${filesHtml}
                </ul>
            </div>
        </div>
    `;

    modal.show();
}

function updatePagination(result) {
    const pagination = document.getElementById('pagination');
    
    if (result.totalPages <= 1) {
        pagination.innerHTML = '';
        return;
    }

    let paginationHtml = '<nav><ul class="pagination">';
    
    // Previous button
    if (result.page > 1) {
        paginationHtml += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="searchCollections(${result.page - 1})">${t('previous')}</a>
            </li>
        `;
    }

    // Page numbers
    const startPage = Math.max(1, result.page - 2);
    const endPage = Math.min(result.totalPages, result.page + 2);

    for (let i = startPage; i <= endPage; i++) {
        paginationHtml += `
            <li class="page-item ${i === result.page ? 'active' : ''}">
                <a class="page-link" href="#" onclick="searchCollections(${i})">${i}</a>
            </li>
        `;
    }

    // Next button
    if (result.page < result.totalPages) {
        paginationHtml += `
            <li class="page-item">
                <a class="page-link" href="#" onclick="searchCollections(${result.page + 1})">${t('next')}</a>
            </li>
        `;
    }

    paginationHtml += '</ul></nav>';
    pagination.innerHTML = paginationHtml;
}

function clearFilters() {
    document.getElementById('searchText').value = '';
    document.getElementById('categoryFilter').value = '';
    document.getElementById('tag1Filter').value = '';
    document.getElementById('tag2Filter').value = '';
    document.getElementById('tag3Filter').value = '';
    document.getElementById('inventoryStatusFilter').value = '';
    searchCollections();
}

function showLoading(show) {
    document.getElementById('loading').style.display = show ? 'block' : 'none';
}

function hideError() {
    document.getElementById('error').style.display = 'none';
}

function showError(message) {
    const errorElement = document.getElementById('error');
    errorElement.textContent = message;
    errorElement.style.display = 'block';
}

function toggleLanguage() {
    currentLanguage = currentLanguage === 'ja' ? 'en' : 'ja';
    updateLanguageDisplay();
}

function updateLanguageDisplay() {
    // This is a simplified implementation - in a real app you might want more sophisticated i18n
    const elements = document.querySelectorAll('[data-lang]');
    elements.forEach(element => {
        const key = element.getAttribute('data-lang');
        if (translations[currentLanguage][key]) {
            if (element.tagName.toLowerCase() === 'input' && element.type === 'text') {
                element.placeholder = translations[currentLanguage][key];
            } else {
                element.textContent = translations[currentLanguage][key];
            }
        }
    });
    
    // Re-render current results with new language
    if (currentSearchCriteria && Object.keys(currentSearchCriteria).length > 0) {
        searchCollections(currentPage);
    }
}

function t(key) {
    return translations[currentLanguage][key] || key;
}

function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.toString().replace(/[&<>"']/g, function(m) { return map[m]; });
}

// Add event listeners for search on Enter key
document.addEventListener('DOMContentLoaded', function() {
    document.getElementById('searchText').addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            searchCollections();
        }
    });
});