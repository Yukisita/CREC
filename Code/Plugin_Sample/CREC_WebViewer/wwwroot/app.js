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
let projectSettings = {
    projectName: '',
    uuidName: 'ID',
    managementCodeName: 'MC',
    categoryName: 'カテゴリ',
    tag1Name: 'タグ 1',
    tag2Name: 'タグ 2',
    tag3Name: 'タグ 3'
}; // Project settings loaded from .crec file

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
        'next': '次へ',
        'search-field': '検索対象',
        'field-all': '全項目',
        'field-id': 'UUID/ID',
        'field-name': '名称',
        'field-mc': '管理コード',
        'field-category': 'カテゴリー',
        'field-tag': 'タグ',
        'field-location': '場所',
        'search-method': '検索方法',
        'method-partial': '部分一致',
        'method-prefix': '前方一致',
        'method-suffix': '後方一致',
        'method-exact': '完全一致',
        'all-status': '全状況',
        'page-size': '表示件数',
        'search-button': '検索',
        'clear-button': 'クリア',
        'close': '閉じる'
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
        'next': 'Next',
        'search-field': 'Search Field',
        'field-all': 'All Fields',
        'field-id': 'UUID/ID',
        'field-name': 'Name',
        'field-mc': 'MC',
        'field-category': 'Category',
        'field-tag': 'Tag',
        'field-location': 'Location',
        'search-method': 'Search Method',
        'method-partial': 'Partial',
        'method-prefix': 'Prefix',
        'method-suffix': 'Suffix',
        'method-exact': 'Exact',
        'all-status': 'All Status',
        'page-size': 'Page Size',
        'search-button': 'Search',
        'clear-button': 'Clear',
        'close': 'Close'
    }
};

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});

// Update UI language
function updateUILanguage() {
    const lang = currentLanguage;
    
    // Update all elements with data-lang attribute
    document.querySelectorAll('[data-lang]').forEach(element => {
        const key = element.getAttribute('data-lang');
        const translation = translations[lang][key];
        
        if (translation) {
            element.textContent = translation;
        }
    });
}

async function initializeApp() {
    try {
        console.log('Initializing app...');

        // Load project settings first
        await loadProjectSettings();

        // Update UI with custom labels
        updateUILabels();
        
        // Update UI language
        updateUILanguage();

        // Add Enter key listener for search
        const searchTextElement = document.getElementById('searchText');
        if (searchTextElement) {
            searchTextElement.addEventListener('keypress', function (e) {
                if (e.key === 'Enter') {
                    searchCollections();
                }
            });
        }

        // No longer need to load filters since we have static dropdowns
        await searchCollections();
        console.log('App initialized successfully');
    }catch (error) {
        console.error('Error initializing app:', error);
        showError('Failed to initialize application: ' + error.message);
    }
}

// Load project settings from API
async function loadProjectSettings() {
    try {
        const response = await fetch('/api/ProjectSettings');
        if (response.ok) {
            const settings = await response.json();
            projectSettings = {
                projectName: settings.projectName || '',
                objectNameLabel: settings.objectNameLabel || (currentLanguage === 'ja' ? '名称' : 'Name'),
                uuidName: settings.uuidName || 'ID',
                managementCodeName: settings.managementCodeName || 'MC',
                categoryName: settings.categoryName || (currentLanguage === 'ja' ? 'カテゴリ' : 'Category'),
                tag1Name: settings.tag1Name || (currentLanguage === 'ja' ? 'タグ 1' : 'Tag 1'),
                tag2Name: settings.tag2Name || (currentLanguage === 'ja' ? 'タグ 2' : 'Tag 2'),
                tag3Name: settings.tag3Name || (currentLanguage === 'ja' ? 'タグ 3' : 'Tag 3')
            };
            console.log('Project settings loaded:', projectSettings);
        }
    } catch (error) {
        console.warn('Could not load project settings, using defaults:', error);
        // Keep default values already initialized
    }
}

// Update UI labels with custom values from project settings
function updateUILabels() {
    // Update search field dropdown options
    const searchFieldElement = document.getElementById('searchField');
    if (searchFieldElement) {
        // Remember current selection
        const currentValue = searchFieldElement.value;
        
        // Clear all options
        searchFieldElement.innerHTML = '';
        
        // Add "All Fields" option - SearchField.All = 0
        const allFieldsOption = document.createElement('option');
        allFieldsOption.value = '0';
        allFieldsOption.text = currentLanguage === 'ja' ? 'すべてのフィールド' : 'All Fields';
        searchFieldElement.appendChild(allFieldsOption);
        
        // Add ID option - SearchField.ID = 1
        const idOption = document.createElement('option');
        idOption.value = '1';
        idOption.text = projectSettings.uuidName;
        searchFieldElement.appendChild(idOption);
        
        // Add Name option - SearchField.Name = 2
        const nameOption = document.createElement('option');
        nameOption.value = '2';
        nameOption.text = projectSettings.objectNameLabel || (currentLanguage === 'ja' ? '名称' : 'Name');
        searchFieldElement.appendChild(nameOption);
        
        // Add MC option - SearchField.ManagementCode = 3
        const mcOption = document.createElement('option');
        mcOption.value = '3';
        mcOption.text = projectSettings.managementCodeName;
        searchFieldElement.appendChild(mcOption);
        
        // Add Category option - SearchField.Category = 4
        const categoryOption = document.createElement('option');
        categoryOption.value = '4';
        categoryOption.text = projectSettings.categoryName;
        searchFieldElement.appendChild(categoryOption);
        
        // Add Tag (all) option - SearchField.Tag = 5
        const tagAllOption = document.createElement('option');
        tagAllOption.value = '5';
        tagAllOption.text = currentLanguage === 'ja' ? 'タグ (全て)' : 'Tags (All)';
        searchFieldElement.appendChild(tagAllOption);
        
        // Add individual tag options - SearchField.Tag1/2/3 = 6/7/8
        const tag1Option = document.createElement('option');
        tag1Option.value = '6';
        tag1Option.text = projectSettings.tag1Name;
        searchFieldElement.appendChild(tag1Option);
        
        const tag2Option = document.createElement('option');
        tag2Option.value = '7';
        tag2Option.text = projectSettings.tag2Name;
        searchFieldElement.appendChild(tag2Option);
        
        const tag3Option = document.createElement('option');
        tag3Option.value = '8';
        tag3Option.text = projectSettings.tag3Name;
        searchFieldElement.appendChild(tag3Option);
        
        // Add Location option - SearchField.Location = 9
        const locationOption = document.createElement('option');
        locationOption.value = '9';
        locationOption.text = currentLanguage === 'ja' ? '場所' : 'Location';
        searchFieldElement.appendChild(locationOption);
        
        // Restore previous selection if still valid
        if (currentValue && Array.from(searchFieldElement.options).some(opt => opt.value === currentValue)) {
            searchFieldElement.value = currentValue;
        }
    }
    
    // Update page title if project name is available
    if (projectSettings.projectName) {
        document.title = `${projectSettings.projectName} - CREC Web Viewer`;
        const titleElement = document.querySelector('h1');
        if (titleElement) {
            titleElement.textContent = projectSettings.projectName;
        }
    }
}

// Search collections
async function searchCollections(page = 1) {
    try {
        currentPage = page;
        
        // Get page size element - use safer approach with default value
        const pageSizeElement = document.getElementById('pageSize');
        currentPageSize = pageSizeElement ? parseInt(pageSizeElement.value) : 20;
        
        // Get all search filter elements with defensive approach
        const searchTextElement = document.getElementById('searchText');
        const searchFieldElement = document.getElementById('searchField');
        const searchMethodElement = document.getElementById('searchMethod');
        const inventoryStatusElement = document.getElementById('inventoryStatusFilter');
        
        // Build criteria object with safe access and default values
        const criteria = {
            searchText: searchTextElement ? searchTextElement.value : '',
            searchField: searchFieldElement ? (parseInt(searchFieldElement.value) || 0) : 0,
            searchMethod: searchMethodElement ? (parseInt(searchMethodElement.value) || 0) : 0,
            inventoryStatus: inventoryStatusElement ? (inventoryStatusElement.value || null) : null,
            page: currentPage,
            pageSize: currentPageSize
        };

        currentSearchCriteria = criteria;
        console.log('Search criteria:', criteria);

        showLoading(true);
        hideError();

        const queryParams = new URLSearchParams();
        Object.keys(criteria).forEach(key => {
            if (criteria[key] !== null && criteria[key] !== '') {
                queryParams.append(key, criteria[key]);
            }
        });

        console.log('Query params:', queryParams.toString());
        const response = await fetch(`/api/collections/search?${queryParams}`);
        console.log('Response status:', response.status);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        console.log('Search result:', result);
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

    // Debug: log the collection object to see what properties it has
    console.log('Collection object:', collection);
    console.log('Collection ID:', collection.collectionID);

    const inventoryStatusText = getInventoryStatusText(collection.collectionInventoryStatus);
    const inventoryBadgeClass = getInventoryStatusBadgeClass(collection.collectionInventoryStatus);

    // Build tag HTML - display each tag on a separate line like category
    let tagsHtml = '';
    if (collection.collectionTag1 && collection.collectionTag1 !== ' - ') {
        const tag1Label = projectSettings.tag1Name || (currentLanguage === 'ja' ? 'タグ 1' : 'Tag 1');
        tagsHtml += `<small class="text-muted">${tag1Label}: ${escapeHtml(collection.collectionTag1)}</small><br>`;
    }
    if (collection.collectionTag2 && collection.collectionTag2 !== ' - ') {
        const tag2Label = projectSettings.tag2Name || (currentLanguage === 'ja' ? 'タグ 2' : 'Tag 2');
        tagsHtml += `<small class="text-muted">${tag2Label}: ${escapeHtml(collection.collectionTag2)}</small><br>`;
    }
    if (collection.collectionTag3 && collection.collectionTag3 !== ' - ') {
        const tag3Label = projectSettings.tag3Name || (currentLanguage === 'ja' ? 'タグ 3' : 'Tag 3');
        tagsHtml += `<small class="text-muted">${tag3Label}: ${escapeHtml(collection.collectionTag3)}</small><br>`;
    }

    // Use the collection ID (which is the folder name) for the thumbnail URL
    const collectionId = collection.collectionID || 'unknown';
    const thumbnailUrl = `/api/Files/thumbnail/${encodeURIComponent(collectionId)}`;
    console.log('Loading thumbnail from:', thumbnailUrl, 'for collection ID:', collectionId);
    
    const thumbnailHtml = `
        <div style="position: relative;">
            <img src="${thumbnailUrl}" 
                 class="card-img-top thumbnail-image"
                 alt="Thumbnail"
                 data-collection-id="${escapeHtml(collectionId)}"
                 style="display: block;"
                 onerror="console.error('Failed to load thumbnail for collection ID: ${escapeHtml(collectionId)}'); console.error('Thumbnail URL was: ${thumbnailUrl}'); this.style.display='none'; this.nextElementSibling.style.display='flex';">
            <div class="thumbnail-placeholder" style="display: none;">
                <i class="bi bi-image display-4"></i>
                <br><small>${t('no-thumbnail')}</small>
            </div>
        </div>
    `;

    colDiv.innerHTML = `
        <div class="card h-100 collection-card">
            ${thumbnailHtml}
            <div class="card-body">
                <h6 class="card-title">${escapeHtml(collection.collectionName)}</h6>
                <p class="card-text">
                    <small class="text-muted">${projectSettings.uuidName}: ${escapeHtml(collection.collectionID)}</small><br>
                    <small class="text-muted">${projectSettings.categoryName}: ${escapeHtml(collection.collectionCategory)}</small><br>
                    ${tagsHtml}
                    ${collection.collectionCurrentInventory !== null ? 
                        `<small class="text-muted">${t('inventory')}: ${collection.collectionCurrentInventory}</small><br>` : ''}
                    <span class="badge ${inventoryBadgeClass}">${inventoryStatusText}</span>
                </p>
            </div>
            <div class="card-footer">
                <button class="btn btn-primary btn-sm w-100" onclick="showCollectionDetails('${escapeHtml(collection.collectionID)}')">
                    <i class="bi bi-eye"></i> ${t('view-details')}
                </button>
            </div>
        </div>
    `;
    
    // Attach the error handler after the element is created
    const img = colDiv.querySelector('.thumbnail-image');
    if (img) {
        const collectionId = img.getAttribute('data-collection-id');
        img.onerror = function() {
            console.error('Failed to load thumbnail for collection ID:', collectionId);
            console.error('Thumbnail URL was:', this.src);
            this.style.display='none';
            this.nextElementSibling.style.display='flex';
        };
    }

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

    let currentImageIndex = 0;
    const images = collection.imageFiles || [];
    
    const imagesHtml = images.length > 0 
        ? `
            <div class="image-carousel">
                <img id="carouselImage" src="/api/File/${encodeURIComponent(collection.collectionID)}/${encodeURIComponent(images[0])}" 
                     class="img-fluid rounded" 
                     alt="${escapeHtml(images[0])}" 
                     style="max-height: 400px; max-width: 100%; object-fit: contain; display: block; margin: 0 auto;"
                     onerror="this.onerror=null; this.src='data:image/svg+xml,%3Csvg xmlns=\'http://www.w3.org/2000/svg\' width=\'200\' height=\'200\'%3E%3Crect width=\'200\' height=\'200\' fill=\'%23ddd\'/%3E%3Ctext x=\'50%25\' y=\'50%25\' dominant-baseline=\'middle\' text-anchor=\'middle\' font-family=\'sans-serif\' font-size=\'16\' fill=\'%23999\'%3EImage not found%3C/text%3E%3C/svg%3E';">
                <div class="carousel-controls text-center mt-3" style="display: flex; justify-content: center; align-items: center; gap: 20px;">
                    <button id="prevImage" class="btn btn-outline-secondary" style="font-size: 20px; padding: 8px 20px;">◀</button>
                    <span id="imageCounter" style="font-size: 16px; min-width: 60px;">1 / ${images.length}</span>
                    <button id="nextImage" class="btn btn-outline-secondary" style="font-size: 20px; padding: 8px 20px;">▶</button>
                </div>
                <p id="imageName" class="small text-muted text-center mt-2">${escapeHtml(images[0])}</p>
            </div>
          `
        : `<p class="text-muted">${t('no-images')}</p>`;

    const filesHtml = collection.otherFiles.length > 0
        ? collection.otherFiles.map(file => `
            <li class="list-group-item d-flex justify-content-between align-items-center">
                ${escapeHtml(file)}
                <a href="/api/File/data/${encodeURIComponent(collection.collectionID)}/${encodeURIComponent(file)}" 
                   class="btn btn-sm btn-outline-primary" target="_blank">
                    <i class="bi bi-download"></i>
                </a>
            </li>
          `).join('')
        : `<p class="text-muted">No files</p>`;

    modalBody.innerHTML = `
        <div class="row">
            <div class="col-md-6">
                <h6>${projectSettings.uuidName}</h6>
                <p>${escapeHtml(collection.collectionID)}</p>
                
                <h6>${projectSettings.managementCodeName}</h6>
                <p>${escapeHtml(collection.collectionMC)}</p>
                
                <h6>${projectSettings.categoryName}</h6>
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
                <div>
                    ${collection.collectionTag1 && collection.collectionTag1 !== ' - ' ? `<p>${projectSettings.tag1Name || (currentLanguage === 'ja' ? 'タグ 1' : 'Tag 1')}: ${escapeHtml(collection.collectionTag1)}</p>` : ''}
                    ${collection.collectionTag2 && collection.collectionTag2 !== ' - ' ? `<p>${projectSettings.tag2Name || (currentLanguage === 'ja' ? 'タグ 2' : 'Tag 2')}: ${escapeHtml(collection.collectionTag2)}</p>` : ''}
                    ${collection.collectionTag3 && collection.collectionTag3 !== ' - ' ? `<p>${projectSettings.tag3Name || (currentLanguage === 'ja' ? 'タグ 3' : 'Tag 3')}: ${escapeHtml(collection.collectionTag3)}</p>` : ''}
                    ${(!collection.collectionTag1 || collection.collectionTag1 === ' - ') && 
                      (!collection.collectionTag2 || collection.collectionTag2 === ' - ') && 
                      (!collection.collectionTag3 || collection.collectionTag3 === ' - ') ? `<p>${t('not-set')}</p>` : ''}
                </div>
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
    
    // Set up carousel controls if images exist
    if (images.length > 0) {
        // Wait for modal to be shown before setting up carousel
        const modalElement = document.getElementById('collectionModal');
        modalElement.addEventListener('shown.bs.modal', () => {
            const carouselImage = document.getElementById('carouselImage');
            const imageCounter = document.getElementById('imageCounter');
            const imageName = document.getElementById('imageName');
            const prevBtn = document.getElementById('prevImage');
            const nextBtn = document.getElementById('nextImage');
            
            // Check if all elements exist before proceeding
            if (!carouselImage || !imageCounter || !imageName || !prevBtn || !nextBtn) {
                return;
            }
            
            function updateImage() {
                carouselImage.src = `/api/File/${encodeURIComponent(collection.collectionID)}/${encodeURIComponent(images[currentImageIndex])}`;
                carouselImage.alt = images[currentImageIndex];
                imageCounter.textContent = `${currentImageIndex + 1} / ${images.length}`;
                imageName.textContent = images[currentImageIndex];
            }
            
            prevBtn.addEventListener('click', () => {
                currentImageIndex = (currentImageIndex - 1 + images.length) % images.length;
                updateImage();
            });
            
            nextBtn.addEventListener('click', () => {
                currentImageIndex = (currentImageIndex + 1) % images.length;
                updateImage();
            });
            
            // Keyboard navigation
            const keyHandler = (e) => {
                if (e.key === 'ArrowLeft') {
                    currentImageIndex = (currentImageIndex - 1 + images.length) % images.length;
                    updateImage();
                } else if (e.key === 'ArrowRight') {
                    currentImageIndex = (currentImageIndex + 1) % images.length;
                    updateImage();
                }
            };
            
            document.addEventListener('keydown', keyHandler);
            
            // Clean up event listener when modal is closed
            modalElement.addEventListener('hidden.bs.modal', () => {
                document.removeEventListener('keydown', keyHandler);
            }, { once: true });
        }, { once: true });
    }
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
    const searchTextElement = document.getElementById('searchText');
    const searchFieldElement = document.getElementById('searchField');
    const searchMethodElement = document.getElementById('searchMethod');
    const inventoryStatusElement = document.getElementById('inventoryStatusFilter');
    
    if (searchTextElement) searchTextElement.value = '';
    if (searchFieldElement) searchFieldElement.value = '0';
    if (searchMethodElement) searchMethodElement.value = '0';
    if (inventoryStatusElement) inventoryStatusElement.value = '';
    
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
            const tagName = element.tagName.toLowerCase();
            
            if (tagName === 'input' && element.type === 'text') {
                // Update placeholder for text inputs
                element.placeholder = translations[currentLanguage][key];
            } else if (tagName === 'option') {
                // Update text content for option elements (remove bilingual format)
                element.textContent = translations[currentLanguage][key];
            } else {
                // Update text content for other elements
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
    return text.toString().replace(/[&<>"']/g, function (m) { return map[m]; });
}