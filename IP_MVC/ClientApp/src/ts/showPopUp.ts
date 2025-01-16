import {end} from "@popperjs/core";

const popupButton = document.getElementById('popupButton')!;
const popupOverlay = document.getElementById('popupOverlay')!;
const closePopupButton = document.getElementById('closePopup')!;
const submitButton = document.getElementById('createButton')!;
const startDate = document.getElementById('NewStartDateInput') as HTMLInputElement;
const endDate = document.getElementById('NewEndDateInput')as HTMLInputElement;
const stopProjectSessionButton = document.getElementById('stopProjectSessionButton')!;

export function showPopup(open: boolean, autoClose: boolean = false) {
    if (open){
        popupOverlay.style.display = 'block';
        if (startDate != null && endDate != null){
            const currentDate = new Date().toISOString().slice(0, 16); 
            startDate.value = currentDate;
            endDate.value = currentDate;
        }
        if (autoClose) {
            setTimeout(() => showPopup(false), 60000);
        }
    } else {
        popupOverlay.style.display = 'none';
    }
}

popupButton.addEventListener('click', () => showPopup(true));
closePopupButton.addEventListener('click', () => showPopup(false));
submitButton?.addEventListener('click', () => showPopup(false));
popupOverlay.addEventListener('click', function (event) {
    if (event.target === popupOverlay) {
        showPopup(false);
    }
});
stopProjectSessionButton.addEventListener('click', () => showPopup(true, true));