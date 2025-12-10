const apiBase = "api.php";

const welcome = document.getElementById("welcome");
const surveyWrap = document.getElementById("surveyWrap");
const results = document.getElementById("results");
const form = document.getElementById("surveyForm");

const startBtn = document.getElementById("startBtn");
const clearBtn = document.getElementById("clearBtn");
const submitBtn = document.getElementById("submitBtn");
const retakeBtn = document.getElementById("retakeBtn");
const finishBtn = document.getElementById("finishBtn");

let surveyData = [];

(async function preloadSurvey() {
    try {
        const res = await fetch(`${apiBase}?action=get_survey`);
        const data = await res.json();
        if (data.success && data.categories) {
            surveyData = data.categories;
            updateEstimatedTime();
        }
    } catch (err) {
        console.error(err);
    }
})();

startBtn.addEventListener("click", async () => {
    welcome.style.display = "none";
    surveyWrap.style.display = "block";
    await loadSurvey();
});

clearBtn.addEventListener("click", () => {
    form.querySelectorAll("input[type=radio]").forEach(r => r.checked = false);
});

retakeBtn.addEventListener("click", () => {
    results.style.display = "none";
    surveyWrap.style.display = "block";
    form.querySelectorAll("input[type=radio]").forEach(r => r.checked = false);
});

finishBtn.addEventListener("click", () => {
    results.style.display = "none";
    welcome.style.display = "block";
});

// Load survey from API
async function loadSurvey() {
    form.innerHTML = "<p>Loading questions...</p>";
    try {
        const res = await fetch(`${apiBase}?action=get_survey`);
        const data = await res.json();
        if (data.success && data.categories) {
            surveyData = data.categories;
            renderSurvey(surveyData);
            updateEstimatedTime();
        } else {
            form.innerHTML = "<p>Failed to load survey questions.</p>";
        }
    } catch (err) {
        console.error(err);
        form.innerHTML = "<p>Error loading survey.</p>";
    }
}

// Render survey questions
function renderSurvey(categories) {
    form.innerHTML = "";

    // Detect if small screen (≤ 768px)
    const isSmallScreen = window.matchMedia("(max-width: 768px)").matches;

    // Define label sets
    const labels = isSmallScreen
        ? ["SD", "D", "N", "A", "SA"]
        : ["Strongly Disagree", "Disagree", "Neutral", "Agree", "Strongly Agree"];

    const colors = ["#e74c3c", "#e67e22", "#9ca3af", "#2ecc71", "#27ae60"];
    const sizes = ["size-large", "size-medium", "size-small", "size-medium", "size-large"];

    categories.forEach(cat => {
        const header = document.createElement("div");
        header.innerHTML = `<h3 style="margin-top:20px">${cat.name}</h3>`;
        form.appendChild(header);

        cat.questions.forEach((q, index) => {
            const qBlock = document.createElement("div");
            qBlock.className = "question-block";

            const left = document.createElement("div");
            left.className = "q-left";
            left.innerHTML = `<h4>${q.question_text}</h4><p>${cat.name} — Question ${index + 1}</p>`;
            qBlock.appendChild(left);

            const right = document.createElement("div");
            right.className = "q-right";
            const row = document.createElement("div");
            row.className = "radio-row";

            for (let i = 0; i < 5; i++) {
                const lbl = document.createElement("label");
                lbl.className = "radio-btn " + sizes[i];

                const input = document.createElement("input");
                input.type = "radio";
                input.name = "q_" + q.id;
                input.value = i + 1;
                input.dataset.color = colors[i];
                lbl.appendChild(input);

                const txt = document.createElement("div");
                txt.className = "radio-label";
                txt.textContent = labels[i];
                txt.style.borderColor = colors[i];
                txt.style.setProperty("--border-color", colors[i]);
                txt.style.setProperty("--fill-color", colors[i]);
                lbl.appendChild(txt);

                row.appendChild(lbl);
            }

            right.appendChild(row);
            qBlock.appendChild(right);
            form.appendChild(qBlock);
        });
    });
}

window.addEventListener("resize", () => {
    if (surveyData.length > 0) {
        const answers = {};
        form.querySelectorAll("input[type=radio]:checked").forEach(input => {
            answers[input.name] = input.value;
        });

        renderSurvey(surveyData);

        Object.keys(answers).forEach(name => {
            const el = form.querySelector(`input[name="${name}"][value="${answers[name]}"]`);
            if (el) el.checked = true;
        });
    }
});

// Dim the whole question block once answered
document.addEventListener("change", (e) => {
    if (e.target.type === "radio") {
        const qBlock = e.target.closest(".question-block");
        if (qBlock) {
            qBlock.classList.add("answered");
        }
    }
});


// Submit survey
submitBtn.addEventListener("click", async () => {
    const allQ = form.querySelectorAll(".question-block");
    if (allQ.length === 0) {
        alert("Please put questions first.");
        return;
    }
    const missing = [];
    allQ.forEach(qb => {
        const name = qb.querySelector("input[type=radio]").name;
        const checked = form.querySelector(`input[name="${name}"]:checked`);
        if (!checked) missing.push(name);
    });
    
    if (missing.length > 0) { alert("Please answer all questions before submitting."); return; }

    const payload = {
        categories: surveyData.map(cat => ({
            id: cat.id,
            questions: cat.questions.map(q => {
                const val = form.querySelector(`input[name="q_${q.id}"]:checked`).value;
                return { question_id: q.id, rating: parseInt(val) };
            })
        }))
    };

    try {
        const res = await fetch(`${apiBase}?action=submit_survey`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        if (data.success) {
            showResults(payload);
        } else { alert("Submit failed: " + data.error); }
    } catch (err) { console.error(err); alert("Error submitting survey."); }
});

// Show results with question-level and category-level breakdown
function showResults(payload) {
    surveyWrap.style.display = "none";
    results.style.display = "block";

    const categoryList = document.getElementById("categoryList");
    categoryList.innerHTML = "";

    let categoryAvgs = []; // store each category's average

    // Loop through each category
    payload.categories.forEach(cat => {
        let catSum = 0;

        cat.questions.forEach(q => {
            catSum += q.rating; // sum ratings in the category
        });

        const catAvg = (catSum / cat.questions.length).toFixed(2);
        categoryAvgs.push(parseFloat(catAvg));

        // Display category average
        const li = document.createElement("li");
        li.style.fontWeight = "400";
        li.textContent = `${surveyData.find(c => c.id === cat.id).name}: ${catAvg} / 5`;
        categoryList.appendChild(li);
    });

    // Overall satisfaction = average of all category averages
    const overall = (categoryAvgs.reduce((a, b) => a + b, 0) / categoryAvgs.length).toFixed(2);
    document.getElementById("overallScore").textContent = overall;

    // Also show overall at the bottom of category list
    const liOverall = document.createElement("li");
    liOverall.style.fontWeight = "700";
    liOverall.textContent = `Overall Score: ${overall} / 5`;
    categoryList.appendChild(liOverall);

    // Summary text
    let text = "";
    if (overall >= 4.5) text = "Excellent satisfaction!";
    else if (overall >= 3.5) text = "Good satisfaction.";
    else if (overall >= 2.5) text = "Average satisfaction — consider addressing specific concerns.";
    else text = "Low satisfaction. Needs improvement.";

    document.getElementById("resultText").textContent = `Your overall satisfaction score is ${overall}/5. ${text}`;
}

function updateEstimatedTime() {
    if (!surveyData || surveyData.length === 0) return;

    // Count total questions
    const totalQuestions = surveyData.reduce((sum, cat) => sum + cat.questions.length, 0);

    // Estimate: 30 seconds per question
    const secondsPerQuestion = 30;
    const totalSeconds = totalQuestions * secondsPerQuestion;
    const minutes = Math.ceil(totalSeconds / 60);

    // Update the div
    const estDiv = document.getElementById("estimatedTime");
    if (estDiv) {
        estDiv.textContent = `Estimated time: ${minutes} minute${minutes > 1 ? 's' : ''}`;
    }
}

// Dynamically set fill color for each input
document.addEventListener("input", (e) => {
    if (e.target.type === "radio") {
        const color = e.target.dataset.color;
        e.target.nextElementSibling.style.setProperty("--fill-color", color);
    }
});

//Dark Mode
	const link = document.getElementById("themeStylesheet");
    const switchBtn = document.getElementById("switchButton");

    // Checks if a "theme" was saved
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme) {
      link.href = savedTheme;
    }

    // Toggle theme and save choice
    switchBtn.addEventListener("click", function() {
      if (link.href.includes("user.css")) {
        link.href = "userdark.css";
        localStorage.setItem("theme", "userdark.css");
      } else {
        link.href = "user.css";
        localStorage.setItem("theme", "user.css");
      }
    });