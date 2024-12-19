// 'quantity-input' input tag must have data-max-stock attribute

const quantityInput = document.getElementById('quantity-input');
const increaseBtn = document.getElementById('quantity-input__increase-btn');
const decreaseBtn = document.getElementById('quantity-input__decrease-btn');
const warning = document.getElementById('quantity-input__quantity-warning');
const maxStock = parseInt(quantityInput.dataset.maxStock);

function updateButtons() {
    const value = parseInt(quantityInput.value);
    decreaseBtn.classList.toggle('quantity-input__button--disabled', value <= 1);
    increaseBtn.classList.toggle('quantity-input__button--disabled', value >= maxStock);
    warning.style.display = value >= maxStock ? 'block' : 'none';
}

increaseBtn.addEventListener('click', () => {
    if (parseInt(quantityInput.value) < maxStock) {
        quantityInput.value = parseInt(quantityInput.value) + 1;
        updateButtons();
    }
});

decreaseBtn.addEventListener('click', () => {
    if (parseInt(quantityInput.value) > 1) {
        quantityInput.value = parseInt(quantityInput.value) - 1;
        updateButtons();
    }
});

quantityInput.addEventListener('blur', () => {
    let value = parseInt(quantityInput.value);
    if (isNaN(value) || value <= 0) {
        quantityInput.value = 1;
    } else if (value > maxStock) {
        quantityInput.value = maxStock;
    }
    updateButtons();
});

updateButtons();