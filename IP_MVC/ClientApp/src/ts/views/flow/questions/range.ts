window.addEventListener('DOMContentLoaded', () => {
    const selectedValuePlayer1 = document.getElementById('selectedValuePlayer1') as HTMLInputElement;
    selectedValuePlayer1.value = selectedValuePlayer1.dataset.earlierAnswer || '';

    const selectedValuePlayer2 = document.getElementById('selectedValuePlayer2') as HTMLInputElement;
    selectedValuePlayer2.value = selectedValuePlayer2.dataset.earlierAnswer || '';

});

document.getElementById('rangeInputPlayer1')?.addEventListener('input', (e) => {
    const selectedValuePlayer1 = document.getElementById('selectedValuePlayer1') as HTMLInputElement;
    selectedValuePlayer1.value = (e.target as HTMLInputElement).value;
});

document.getElementById('rangeInputPlayer2')?.addEventListener('input', (e) => {
    const selectedValuePlayer2 = document.getElementById('selectedValuePlayer2') as HTMLInputElement;
    selectedValuePlayer2.value = (e.target as HTMLInputElement).value;
});