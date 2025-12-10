const API_URL = 'api.php'; // adjust path if needed

// Navigation
const pages = {
    dashboard: document.getElementById('page-dashboard'),
    analytics: document.getElementById('page-analytics'),
    manage: document.getElementById('page-manage'),
    responses: document.getElementById('page-responses'),
};
const nav = {
    dashboard: document.getElementById('nav-dashboard'),
    analytics: document.getElementById('nav-analytics'),
    manage: document.getElementById('nav-manage'),
    responses: document.getElementById('nav-responses')
};
function showPage(key) {
    for (const p in pages) pages[p].style.display = (p === key ? 'block' : 'none');
    document.querySelectorAll('.nav button').forEach(b => b.classList.remove('active'));
    nav[key].classList.add('active');
    if (key === 'dashboard') renderDashboard();
    if (key === 'analytics') renderAnalytics();
    if (key === 'manage') renderManage();
    if (key === 'responses') renderResponses();
}
Object.keys(nav).forEach(k => nav[k].onclick = () => showPage(k));

// Chart instances
let barChart, lineChart, pieChart, analyticsChart;

// utility fetch wrapper
async function api(action, method = 'GET', payload = null) {
    let url = API_URL + '?action=' + encodeURIComponent(action);
    if (method === 'GET' && payload && Object.keys(payload).length) {
        url += '&' + new URLSearchParams(payload).toString();
    }
    const opts = { method };
    if (method !== 'GET' && payload !== null) {
        opts.headers = { 'Content-Type': 'application/json' };
        opts.body = JSON.stringify(payload);
    } else if (method !== 'GET' && method !== 'DELETE' && payload === null) {
        opts.headers = { 'Content-Type': 'application/json' };
        opts.body = '{}';
    }
    const res = await fetch(url, opts);
    const ct = res.headers.get('Content-Type') || '';
    if (ct.includes('application/json')) {
        return res.json();
    }
    return res.text();
}

// get survey structure
async function loadSurvey() {
    const r = await api('get_survey');
    return r.categories || [];
}

// load responses with optional days filter
async function loadResponses(days = null) {
    const params = {};
    if (days && days !== 'all') params.days = days;
    const r = await api('get_responses', 'GET', params);
    return r.responses || [];
}

// render dashboard
async function renderDashboard() {
    const days = document.getElementById('filterTime').value;
    const params = (days && days !== 'all') ? { days } : {};
    const stats = await api('dashboard_stats', 'GET', params);

    // Update stats
    document.getElementById('stat-count').innerText = stats.total;
    document.getElementById('stat-avg').innerText = stats.overall_avg.toFixed(2);
    document.getElementById('stat-last').innerText = stats.last ? new Date(stats.last).toLocaleString() : '—';

    // Populate category filter
    const catSelect = document.getElementById('filterCategory');
    catSelect.innerHTML = '<option value="all">All Categories</option>' +
        (stats.categories || []).map(c => `<option value="${c.title}">${c.title}</option>`).join('');

    // Prepare initial bar chart (all categories)
    const allLabels = (stats.categories || []).map(c => c.title);
    const allVals = (stats.categories || []).map(c => Number(parseFloat(c.avgv).toFixed(2)));

    if (barChart) barChart.destroy();
    barChart = new Chart(document.getElementById('barChart'), {
        type: 'bar',
        data: { labels: allLabels, datasets: [{ label: 'Avg', data: allVals, backgroundColor: '#1f6fb2' }] },
        options: { scales: { y: { beginAtZero: true, max: 5 } }, plugins: { legend: { display: false } } }
    });

    // Category filter change handler
    catSelect.onchange = async () => {
        const selectedCat = catSelect.value;
        if (selectedCat === 'all') {
            barChart.data.labels = allLabels;
            barChart.data.datasets[0].data = allVals;
            barChart.update();
            return;
        }

        // Find average for the selected category
        let sum = 0, count = 0;
        (stats.categories || []).forEach(c => {
            if (c.title === selectedCat) {
                sum += parseFloat(c.avgv);
                count++;
            }
        });
        const avg = count ? sum / count : 0;

        barChart.data.labels = [selectedCat];
        barChart.data.datasets[0].data = [avg];
        barChart.update();
    };

    // Trend line chart
    const trendLabels = (stats.trend || []).map(t => t.d);
    const trendVals = (stats.trend || []).map(t => Number(parseFloat(t.avgv).toFixed(2)));
    if (lineChart) lineChart.destroy();
    lineChart = new Chart(document.getElementById('lineChart'), {
        type: 'line',
        data: { labels: trendLabels, datasets: [{ label: 'Overall Avg', data: trendVals, borderColor: '#1f8f7a', fill: false }] },
        options: { scales: { y: { beginAtZero: true, max: 5 } }, plugins: { legend: { display: false } } }
    });

    // Pie chart
    if (pieChart) pieChart.destroy();
    pieChart = new Chart(document.getElementById('pieChart'), {
        type: 'pie',
        data: { labels: allLabels, datasets: [{ data: allVals, backgroundColor: ['#1f6fb2', '#1f8f7a', '#f59e0b', '#e11d48', '#8b5cf6'] }] },
        options: { plugins: { legend: { position: 'bottom' } } }
    });
}


// render analytics
// render analytics with number of respondents per day
async function renderAnalytics() {
    const days = document.getElementById('respFilterTime').value;
    const responses = await loadResponses(days === 'all' ? null : days);
    console.log("📊 responses:", responses);

    // ===== Overall bar chart =====
    const map = {};
    responses.forEach(r => {
        const d = new Date(r.submitted_at).toLocaleDateString('en-CA', { timeZone: 'Asia/Manila' });
        if (!map[d]) map[d] = { sum: 0, count: 0 };
        map[d].sum += Number(r.overall) || 0;
        map[d].count++;
    });
    const labels = Object.keys(map).sort((a,b)=> new Date(a) - new Date(b));
    const avgVals = labels.map(l => map[l].count ? map[l].sum / map[l].count : 0);
    const countVals = labels.map(l => map[l].count);

    // Destroy old chart if exists
    if (window.analyticsChart instanceof Chart) window.analyticsChart.destroy();

    const ctxBar = document.getElementById('analyticsChart');
    window.analyticsChart = new Chart(ctxBar, {
        type: 'bar',
        data: {
            labels,
            datasets: [
                { label: 'Average Score', data: avgVals, backgroundColor: '#1f6fb2', yAxisID: 'y1' },
                { label: 'Respondents', data: countVals, backgroundColor: '#f59e0b', yAxisID: 'y2' }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y1: { type: 'linear', position: 'left', beginAtZero: true, max: 5, title: { display: true, text: 'Avg Score' } },
                y2: { type: 'linear', position: 'right', beginAtZero: true, title: { display: true, text: 'Number of Respondents' }, grid: { drawOnChartArea: false } }
            },
            plugins: { legend: { position: 'top' } }
        }
    });

    // ===== Category trend line chart =====
    const catMap = {};
    responses.forEach(r => {
        const day = new Date(r.submitted_at).toLocaleDateString('en-CA', { timeZone: 'Asia/Manila' });
        (r.categories || []).forEach(c => {
            const cat = c.category_name;
            if (!catMap[cat]) catMap[cat] = {};
            if (!catMap[cat][day]) catMap[cat][day] = { sum: 0, count: 0 };
            catMap[cat][day].sum += Number(c.avg_rating) || 0;
            catMap[cat][day].count++;
        });
    });

    const allDates = [...new Set(Object.values(catMap).flatMap(obj => Object.keys(obj)))].sort((a,b)=> new Date(a)-new Date(b));
    const colors = ['#1f6fb2','#e11d48','#f59e0b','#1f8f7a','#8b5cf6','#9333ea','#ef4444','#14b8a6'];

    const datasets = Object.keys(catMap).map((cat,i)=>({
        label: cat,
        data: allDates.map(d => catMap[cat][d] ? catMap[cat][d].sum / catMap[cat][d].count : null),
        borderColor: colors[i % colors.length],
        tension: 0.3,
        fill: false,
        spanGaps: true
    }));

    const ctxLine = document.getElementById('categoryTrendChart');
    if (window.categoryTrendChart instanceof Chart) window.categoryTrendChart.destroy();

    window.categoryTrendChart = new Chart(ctxLine, {
        type: 'line',
        data: { labels: allDates, datasets },
        options: {
            responsive: true,
            plugins: {
                legend: { position: 'top' },
                title: { display: true, text: 'Category Average Trend' }
            },
            interaction: { mode: 'nearest', intersect: false },
            scales: {
                x: { title: { display: true, text: 'Date' } },
                y: { beginAtZero: true, max: 5, title: { display: true, text: 'Average Rating' } }
            }
        }
    });
}


// attach filter change
document.getElementById('respFilterTime').onchange = renderAnalytics;



// manage: render categories & questions
async function renderManage() {
    const categories = await loadSurvey();
    const catList = document.getElementById('categoryList');
    catList.innerHTML = '';
    const selectAdd = document.getElementById('selectCategoryToAdd');
    selectAdd.innerHTML = categories.map(c => `<option value="${c.id}">${escapeHtml(c.name)}</option>`).join('');

    categories.forEach(cat => {
        const div = document.createElement('div');
        div.className = 'card';
        div.innerHTML = `
      <div style="display:flex;justify-content:space-between;align-items:center">
        <strong>${escapeHtml(cat.name)}</strong>
        <div>
          <button data-id="${cat.id}" class="edit-cat btn ghost">Edit</button>
          <button data-id="${cat.id}" class="del-cat btn danger">Delete</button>
        </div>
      </div>
      <div style="margin-top:8px">
        <strong>Questions</strong>
        <div id="qlist-${cat.id}" style="margin-top:8px"></div>
      </div>
    `;
        catList.appendChild(div);
        const qlist = div.querySelector(`#qlist-${cat.id}`);
        (cat.questions || []).forEach(q => {
            const row = document.createElement('div');
            row.style.display = 'flex'; row.style.gap = '8px'; row.style.marginBottom = '6px';
            row.innerHTML = `
        <input value="${escapeHtml(q.question_text)}" style="flex:1" />
        <button class="save-q btn ghost" data-qid="${q.id}" data-cid="${cat.id}">Save</button>
        <button class="del-q btn danger" data-qid="${q.id}">Delete</button>
      `;
            qlist.appendChild(row);
        });
    });

    // attach handlers
    document.querySelectorAll('.del-cat').forEach(b => {
        b.onclick = async (e) => {
            if (!confirm('Delete category and questions?')) return;
            await api('delete_category', 'POST', { id: b.dataset.id });
            renderManage();
            renderDashboard();
        };
    });
    document.querySelectorAll('.edit-cat').forEach(b => {
        b.onclick = async (e) => {
            const id = b.dataset.id;
            const newTitle = prompt('New category title:');
            if (!newTitle) return;
            await api('update_category', 'POST', { id, title: newTitle });
            renderManage();
            renderDashboard();
        };
    });
    document.querySelectorAll('.save-q').forEach(b => {
        b.onclick = async (e) => {
            const qid = b.dataset.qid;
            const input = b.parentElement.querySelector('input');
            await api('update_question', 'POST', { id: qid, text: input.value });
            alert('Saved');
            renderManage();
        };
    });
    document.querySelectorAll('.del-q').forEach(b => {
        b.onclick = async (e) => {
            if (!confirm('Delete question?')) return;
            await api('delete_question', 'POST', { id: b.dataset.qid });
            renderManage();
        };
    });
    document.getElementById('clearResponses').onclick = async () => {
        if (!confirm("Are you sure you want to delete ALL responses? This cannot be undone.")) return;

        try {
            // call API to delete all responses
            await api('delete_responses', 'POST', {});
            alert("All responses have been deleted.");
            renderResponses();
            renderDashboard();
            renderAnalytics();
        } catch (err) {
            console.error(err);
            alert("Failed to delete responses.");
        }
    };
}



// responses table
async function renderResponses() {
    const days = document.getElementById('respFilterTime').value;
    const responses = await loadResponses(days === 'all' ? null : days);
    const tbody = document.getElementById('responsesTableBody');
    tbody.innerHTML = '';
    if (!responses.length) {
        tbody.innerHTML = '<tr><td colspan="3">No responses</td></tr>';
        return;
    }
    responses.reverse().forEach(r => {
        const tr = document.createElement('tr');
        const details = (r.categories || []).map(c => `${c.category_name}: ${Number(c.avg_rating || 0).toFixed(2)}`).join(' | ');
        tr.innerHTML = `<td>${new Date(r.submitted_at).toLocaleString()}</td><td>${(r.overall || 0).toFixed(2)}</td><td style="font-size:12px;color:#555">${details}</td>`;
        tbody.appendChild(tr);
    });
}

/* Helpers */
function escapeHtml(s) { return String(s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;'); }

/* CRUD form handlers */
document.getElementById('addCategoryBtn').onclick = async () => {
    const title = document.getElementById('newCategoryName').value.trim();
    if (!title) return alert('Enter title');
    await api('add_category', 'POST', { title });
    document.getElementById('newCategoryName').value = '';
    renderManage(); renderDashboard();
};

document.getElementById('addQuestionBtn').onclick = async () => {
    const catId = document.getElementById('selectCategoryToAdd').value;
    const text = document.getElementById('newQuestionText').value.trim();
    if (!text) return alert('Enter question text');
    await api('add_question', 'POST', { category_id: catId, text });
    document.getElementById('newQuestionText').value = '';
    renderManage(); renderDashboard();
};

/* Filter & export */
document.getElementById('filterTime').onchange = renderDashboard;
document.getElementById('filterCategory').onchange = async () => {
    const cat = document.getElementById('filterCategory').value;
    const days = document.getElementById('filterTime').value;
    if (cat === 'all') return renderDashboard();
    const responses = await loadResponses(days === 'all' ? null : days);
    const map = {};
    responses.forEach(r => {
        (r.categories || []).forEach(c => {
            if (c.category_name === cat) {
                if (!map[c.category_name]) map[c.category_name] = { sum: 0, count: 0 };
                map[c.category_name].sum += c.avg_rating;
                map[c.category_name].count++;
            }
        });
    });
    const labels = Object.keys(map);
    const vals = labels.map(k => map[k].count ? (map[k].sum / map[k].count) : 0);
    if (barChart) barChart.destroy();
    barChart = new Chart(document.getElementById('barChart'), {
        type: 'bar',
        data: { labels, datasets: [{ data: vals, backgroundColor: '#1f6fb2' }] },
        options: { scales: { y: { beginAtZero: true, max: 5 } }, plugins: { legend: { display: false } } }
    });
};

document.getElementById('downloadCSV').onclick = () => {
    const days = document.getElementById('filterTime').value;
    const url = API_URL + '?action=export_csv' + ((days && days !== 'all') ? '&days=' + encodeURIComponent(days) : '');
    window.location = url;
};

document.getElementById('exportReport').onclick = async () => {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();
    doc.setFontSize(16);
    doc.text("Survey Analytics Report", 20, 20);

    const days = document.getElementById('filterTime').value;
    const stats = await api('dashboard_stats', 'GET', (days && days !== 'all') ? { days } : {});
    const responses = await loadResponses((days && days !== 'all') ? days : null);

    doc.setFontSize(12);
    doc.text(`Total responses: ${stats.total}`, 20, 35);
    doc.text(`Overall average: ${stats.overall_avg.toFixed(2)}`, 20, 45);

    // Category averages
    doc.text("Category averages:", 20, 60);
    let y = 70;
    (stats.categories || []).forEach(c => {
        doc.text(`${c.title}: ${Number(c.avgv).toFixed(2)}`, 22, y);
        y += 8;
        if (y > 270) { doc.addPage(); y = 20; }
    });

    // Daily summary (average & respondents per day)
    if (responses.length) {
        // Group by day
        const map = {};
        responses.forEach(r => {
            const dayKey = new Date(r.submitted_at).toLocaleDateString(); // only the date
            if (!map[dayKey]) map[dayKey] = { sum: 0, count: 0 };
            map[dayKey].sum += r.overall || 0;
            map[dayKey].count++;
        });

        const sortedDays = Object.keys(map).sort((a, b) => new Date(a) - new Date(b));
        doc.text("Summary (Avg & Respondents):", 20, y + 8);
        y += 18;

        sortedDays.forEach(day => {
            const avg = map[day].count ? (map[day].sum / map[day].count).toFixed(2) : 0;
            const line = `${day} — Avg: ${avg} — Respondents: ${map[day].count}`;
            doc.text(line, 22, y);
            y += 8;
            if (y > 270) { doc.addPage(); y = 20; }
        });
    }

    doc.text("Generated: " + new Date().toLocaleString(), 20, y + 8);
    doc.save('survey_report.pdf');
};

document.getElementById('respFilterTime').onchange = renderResponses;

/* Init */
(async function init() {
    await renderDashboard();
    renderManage();
    renderResponses();
    renderAnalytics();
})();

