chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.action === "closeTab") {
        // Đóng tab hiện tại
        chrome.tabs.remove(sender.tab.id);
    }
});