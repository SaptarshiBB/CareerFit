// CareerFit - Main JavaScript Application File
(function () {
    "use strict";

    // 1. Dark Mode Theme Controller with localStorage persistence
    const THEME_KEY = "careerfit_theme";
    const themeToggleBtn = document.getElementById("themeToggleBtn");
    const themeIcon = document.getElementById("themeIcon");

    function getPreferredTheme() {
        const storedTheme = localStorage.getItem(THEME_KEY);
        if (storedTheme) {
            return storedTheme;
        }
        return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem(THEME_KEY, theme);

        if (themeIcon) {
            if (theme === "dark") {
                themeIcon.classList.remove("bi-moon-fill");
                themeIcon.classList.add("bi-sun-fill");
            } else {
                themeIcon.classList.remove("bi-sun-fill");
                themeIcon.classList.add("bi-moon-fill");
            }
        }
    }

    // Initialize theme immediately
    const initialTheme = getPreferredTheme();
    applyTheme(initialTheme);

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener("click", function () {
            const currentTheme = document.documentElement.getAttribute("data-theme") || "light";
            const newTheme = currentTheme === "dark" ? "light" : "dark";
            applyTheme(newTheme);
        });
    }

    // 2. Auto-dismiss alerts after 6 seconds
    document.addEventListener("DOMContentLoaded", function () {
        const alerts = document.querySelectorAll(".alert-dismissible");
        alerts.forEach(function (alert) {
            setTimeout(function () {
                const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
                if (bsAlert) bsAlert.close();
            }, 6000);
        });
    });
})();
