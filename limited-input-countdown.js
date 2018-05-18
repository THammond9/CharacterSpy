function initCountdownWatcher(elementToWatchAsString, elementToUpdateAsString, maximum) {
    let localElementToUpdateAsString = elementToUpdateAsString;
    let localMaximum = maximum;
    enableCharacterCountdown(elementToWatchAsString);

    function enableCharacterCountdown(elementToWatchAsString) {
        $(elementToWatchAsString).on("change", updateCountdown);
        $(elementToWatchAsString).on("keyup", updateCountdown);
        $(elementToWatchAsString).trigger("keyup");
    }

    function updateCountdown() {
        if (!$(this)) {
            return;
        }

        if (!$(this).val()) {
            $(localElementToUpdateAsString).text(localMaximum + " characters remaining.");
        }

        var remaining = localMaximum - $(this).val().length;
        if (remaining < 0) {
            $(localElementToUpdateAsString).text("Maximum length has been reached");
        } else {
            $(localElementToUpdateAsString).text(remaining + " characters remaining.");
        }
    }
}