// Activities Management JavaScript
// Handles search, filter, and dynamic interactions

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all features
    initializeSearch();
    initializeFilters();
    initializeAnimations();
    initializeFormValidation();
});

// ========== Search Functionality ==========
function initializeSearch() {
    const searchInput = document.getElementById('searchActivity');
    if (!searchInput) return;

    searchInput.addEventListener('input', debounce(function(e) {
        const searchTerm = e.target.value.toLowerCase();
        filterActivities();
    }, 300));
}

// ========== Filter Functionality ==========
function initializeFilters() {
    const clubFilter = document.getElementById('filterClub');
    const statusFilter = document.getElementById('filterStatus');

    if (clubFilter) {
        clubFilter.addEventListener('change', filterActivities);
    }

    if (statusFilter) {
        statusFilter.addEventListener('change', filterActivities);
    }
}

function filterActivities() {
    const searchTerm = document.getElementById('searchActivity')?.value.toLowerCase() || '';
    const selectedClub = document.getElementById('filterClub')?.value || '';
    const selectedStatus = document.getElementById('filterStatus')?.value || '';

    const activityCards = document.querySelectorAll('.activity-card');
    let visibleCount = 0;

    activityCards.forEach(card => {
        const activityName = card.dataset.name?.toLowerCase() || '';
        const clubName = card.dataset.club || '';
        const status = card.dataset.status || '';

        const matchesSearch = activityName.includes(searchTerm);
        const matchesClub = !selectedClub || clubName === selectedClub;
        const matchesStatus = !selectedStatus || status === selectedStatus;

        if (matchesSearch && matchesClub && matchesStatus) {
            card.style.display = '';
            card.classList.add('fade-in');
            visibleCount++;
        } else {
            card.style.display = 'none';
            card.classList.remove('fade-in');
        }
    });

    // Show "no results" message if needed
    updateNoResultsMessage(visibleCount);
}

function updateNoResultsMessage(visibleCount) {
    const grid = document.getElementById('activitiesGrid');
    if (!grid) return;

    let noResultsMsg = document.getElementById('noResultsMessage');

    if (visibleCount === 0) {
        if (!noResultsMsg) {
            noResultsMsg = document.createElement('div');
            noResultsMsg.id = 'noResultsMessage';
            noResultsMsg.className = 'col-12';
            noResultsMsg.innerHTML = `
                <div class="card shadow border-0 text-center py-5 fade-in">
                    <i class="bi bi-search fs-1 text-muted mb-3"></i>
                    <h5 class="text-muted">No activities found</h5>
                    <p class="text-muted">Try adjusting your search or filters</p>
                </div>
            `;
            grid.appendChild(noResultsMsg);
        }
    } else {
        if (noResultsMsg) {
            noResultsMsg.remove();
        }
    }
}

// ========== Animations ==========
function initializeAnimations() {
    // Hover effect for activity cards
    const activityCards = document.querySelectorAll('.activity-item');
    
    activityCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-8px)';
            this.style.transition = 'all 0.3s ease';
        });

        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });

    // Participant row hover effect
    const participantRows = document.querySelectorAll('.participant-row');
    
    participantRows.forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.style.backgroundColor = '#f8f9fa';
            this.style.transform = 'scale(1.01)';
            this.style.transition = 'all 0.2s ease';
        });

        row.addEventListener('mouseleave', function() {
            this.style.backgroundColor = '';
            this.style.transform = 'scale(1)';
        });
    });

    // Fade in animation for cards
    observeElements();
}

function observeElements() {
    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.activity-card').forEach(card => {
        observer.observe(card);
    });
}

// ========== Form Validation ==========
function initializeFormValidation() {
    const createForm = document.getElementById('createActivityForm');
    
    if (createForm) {
        createForm.addEventListener('submit', function(e) {
            const startDate = new Date(document.querySelector('input[name="StartDate"]').value);
            const endDateInput = document.querySelector('input[name="EndDate"]');
            
            if (endDateInput && endDateInput.value) {
                const endDate = new Date(endDateInput.value);
                
                if (endDate <= startDate) {
                    e.preventDefault();
                    showAlert('End date must be after start date', 'danger');
                    return false;
                }
            }

            if (startDate < new Date()) {
                const confirmed = confirm('Start date is in the past. Do you want to continue?');
                if (!confirmed) {
                    e.preventDefault();
                    return false;
                }
            }
        });

        // Real-time date validation
        const startDateInput = document.querySelector('input[name="StartDate"]');
        const endDateInput = document.querySelector('input[name="EndDate"]');

        if (startDateInput && endDateInput) {
            startDateInput.addEventListener('change', validateDates);
            endDateInput.addEventListener('change', validateDates);
        }
    }
}

function validateDates() {
    const startDate = new Date(document.querySelector('input[name="StartDate"]').value);
    const endDateInput = document.querySelector('input[name="EndDate"]');
    
    if (endDateInput && endDateInput.value) {
        const endDate = new Date(endDateInput.value);
        
        if (endDate <= startDate) {
            endDateInput.setCustomValidity('End date must be after start date');
            endDateInput.classList.add('is-invalid');
        } else {
            endDateInput.setCustomValidity('');
            endDateInput.classList.remove('is-invalid');
        }
    }
}

// ========== Utility Functions ==========
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function showAlert(message, type = 'info') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3`;
    alertDiv.style.zIndex = '9999';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(alertDiv);
    
    setTimeout(() => {
        alertDiv.remove();
    }, 5000);
}

// ========== CSS Animations (add to site.css or inline) ==========
const style = document.createElement('style');
style.textContent = `
    .fade-in {
        animation: fadeIn 0.5s ease-in;
    }

    @keyframes fadeIn {
        from {
            opacity: 0;
            transform: translateY(20px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .hover-lift {
        transition: all 0.3s ease;
    }

    .hover-lift:hover {
        box-shadow: 0 10px 30px rgba(0,0,0,0.15) !important;
    }

    .bg-gradient {
        background: linear-gradient(135deg, var(--bs-primary), var(--bs-primary-rgb));
    }

    .participant-row {
        transition: all 0.2s ease;
    }
`;
document.head.appendChild(style);

// Export functions for global use
window.ActivityManagement = {
    filterActivities,
    showAlert
};
