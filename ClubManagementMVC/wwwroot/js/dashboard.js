// Dashboard Charts Initialization using Chart.js
function initializeDashboard(data) {
    console.log('Dashboard data:', data);

    // Chart.js global defaults
    Chart.defaults.font.family = 'system-ui, -apple-system, sans-serif';
    Chart.defaults.color = '#6b7280';

    // 1. Top Clubs Bar Chart (Horizontal)
    initTopClubsChart(data.clubStats.topClubsByMembers);

    // 2. Activities Distribution Doughnut Chart
    initActivitiesDistributionChart(data.clubStats.activitiesByClub);

    // 3. Payment Status Doughnut Chart
    initPaymentStatusChart(data.financialStats);

    // 4. Participation Doughnut Chart
    initParticipationChart(data.activityStats.participationStats);

    // 5. Membership Status Doughnut Chart
    initMembershipStatusChart(data.membershipStats.membershipsByStatus);

    // 6. Monthly Join Trends Line Chart
    initJoinTrendsChart(data.membershipStats.monthlyJoinTrends);
}

// 1. Top Clubs Horizontal Bar Chart
function initTopClubsChart(topClubs) {
    const ctx = document.getElementById('topClubsChart');
    if (!ctx) return;

    const labels = Object.keys(topClubs);
    const values = Object.values(topClubs);

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Số thành viên',
                data: values,
                backgroundColor: [
                    'rgba(102, 126, 234, 0.8)',
                    'rgba(118, 75, 162, 0.8)',
                    'rgba(237, 100, 166, 0.8)',
                    'rgba(255, 154, 158, 0.8)',
                    'rgba(250, 208, 196, 0.8)'
                ],
                borderColor: [
                    'rgb(102, 126, 234)',
                    'rgb(118, 75, 162)',
                    'rgb(237, 100, 166)',
                    'rgb(255, 154, 158)',
                    'rgb(250, 208, 196)'
                ],
                borderWidth: 2,
                borderRadius: 8
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    titleFont: { size: 14, weight: 'bold' },
                    bodyFont: { size: 13 },
                    cornerRadius: 8
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)'
                    }
                },
                y: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });
}

// 2. Activities Distribution Doughnut Chart
function initActivitiesDistributionChart(activitiesByClub) {
    const ctx = document.getElementById('activitiesDistributionChart');
    if (!ctx) return;

    const labels = Object.keys(activitiesByClub);
    const values = Object.values(activitiesByClub);

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: [
                    'rgba(79, 172, 254, 0.8)',
                    'rgba(0, 242, 254, 0.8)',
                    'rgba(250, 112, 154, 0.8)',
                    'rgba(254, 225, 64, 0.8)',
                    'rgba(168, 85, 247, 0.8)'
                ],
                borderColor: '#fff',
                borderWidth: 3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: { size: 12 },
                        usePointStyle: true,
                        pointStyle: 'circle'
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    cornerRadius: 8,
                    callbacks: {
                        label: function(context) {
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((context.parsed / total) * 100).toFixed(1);
                            return `${context.label}: ${context.parsed} (${percentage}%)`;
                        }
                    }
                }
            }
        }
    });
}

// 3. Payment Status Doughnut Chart
function initPaymentStatusChart(financialStats) {
    const ctx = document.getElementById('paymentStatusChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Đã thanh toán', 'Chờ thanh toán'],
            datasets: [{
                data: [financialStats.paidPaymentsCount, financialStats.pendingPaymentsCount],
                backgroundColor: [
                    'rgba(34, 197, 94, 0.8)',
                    'rgba(251, 191, 36, 0.8)'
                ],
                borderColor: '#fff',
                borderWidth: 3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 12,
                        font: { size: 11 },
                        usePointStyle: true
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 10,
                    cornerRadius: 6
                }
            }
        }
    });
}

// 4. Participation Stats Doughnut Chart
function initParticipationChart(participationStats) {
    const ctx = document.getElementById('participationChart');
    if (!ctx) return;

    const labels = Object.keys(participationStats);
    const values = Object.values(participationStats);

    const colorMap = {
        'Attended': 'rgba(34, 197, 94, 0.8)',
        'Registered': 'rgba(59, 130, 246, 0.8)',
        'Absent': 'rgba(239, 68, 68, 0.8)'
    };

    const colors = labels.map(label => colorMap[label] || 'rgba(156, 163, 175, 0.8)');

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: colors,
                borderColor: '#fff',
                borderWidth: 3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: { size: 12 },
                        usePointStyle: true
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    cornerRadius: 8
                }
            }
        }
    });
}

// 5. Membership Status Doughnut Chart
function initMembershipStatusChart(membershipsByStatus) {
    const ctx = document.getElementById('membershipStatusChart');
    if (!ctx) return;

    const labels = Object.keys(membershipsByStatus);
    const values = Object.values(membershipsByStatus);

    const colorMap = {
        'Active': 'rgba(34, 197, 94, 0.8)',
        'Inactive': 'rgba(251, 191, 36, 0.8)',
        'Banned': 'rgba(239, 68, 68, 0.8)'
    };

    const colors = labels.map(label => colorMap[label] || 'rgba(156, 163, 175, 0.8)');

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: colors,
                borderColor: '#fff',
                borderWidth: 3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: { size: 12 },
                        usePointStyle: true
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    cornerRadius: 8,
                    callbacks: {
                        label: function(context) {
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((context.parsed / total) * 100).toFixed(1);
                            return `${context.label}: ${context.parsed} (${percentage}%)`;
                        }
                    }
                }
            }
        }
    });
}

// 6. Monthly Join Trends Line Chart
function initJoinTrendsChart(monthlyTrends) {
    const ctx = document.getElementById('joinTrendsChart');
    if (!ctx) return;

    // Convert YearMonth (202410) to labels
    const sortedKeys = Object.keys(monthlyTrends).sort();
    const labels = sortedKeys.map(key => {
        const year = Math.floor(key / 100);
        const month = key % 100;
        return `Tháng ${month}/${year}`;
    });
    const values = sortedKeys.map(key => monthlyTrends[key]);

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Số thành viên mới',
                data: values,
                borderColor: 'rgb(102, 126, 234)',
                backgroundColor: 'rgba(102, 126, 234, 0.1)',
                borderWidth: 3,
                fill: true,
                tension: 0.4,
                pointRadius: 5,
                pointHoverRadius: 7,
                pointBackgroundColor: 'rgb(102, 126, 234)',
                pointBorderColor: '#fff',
                pointBorderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        font: { size: 12 },
                        usePointStyle: true
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    cornerRadius: 8,
                    titleFont: { size: 13 },
                    bodyFont: { size: 12 }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)'
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            }
        }
    });
}

// Export for global use
window.initializeDashboard = initializeDashboard;
