window.attachAutocompleteScroll = (id, dotNetRef) => {
    const popup = document.querySelector(`#${id} .mud-popover .mud-list`);
    if (!popup) return;

    // Save handler so we can detach later
    const handler = () => {
        if (popup.scrollTop + popup.clientHeight >= popup.scrollHeight - 5) {
            dotNetRef.invokeMethodAsync("OnScrollEnd");
        }
    };

    popup.__scrollHandler = handler;
    popup.addEventListener("scroll", handler);
};

window.detachAutocompleteScroll = (id) => {
    const popup = document.querySelector(`#${id} .mud-popover .mud-list`);
    if (!popup || !popup.__scrollHandler) return;
    popup.removeEventListener("scroll", popup.__scrollHandler);
    delete popup.__scrollHandler;
};
