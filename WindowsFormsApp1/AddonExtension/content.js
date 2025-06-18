(async () => {
    await new Promise(resolve => setTimeout(resolve, 8000)); // Đợi trang load xong
    const html = document.documentElement.outerHTML;
    try {
        const response = await fetch("http://localhost:5000/save", {
            method: "POST",
            body: JSON.stringify({ html: html, url: location.href }),
            headers: { "Content-Type": "application/json" }
        });
        const result = await response.json();
        if (response.ok) {
            // Gửi message tới background.js để đóng tab
            chrome.runtime.sendMessage({ action: "closeTab" });
        } else {
            console.error("Failed to save HTML:", result.error);
        }
    } catch (error) {
        console.error("Error:", error);
    }
})();