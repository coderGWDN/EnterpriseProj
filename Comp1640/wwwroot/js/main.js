// popover
var popoverTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="popover"]')
);
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl);
});

// Gender Select
if (window.location.pathname === "/") {
    const radioBtn1 = document.querySelector("#flexRadioDefault1");
    const radioBtn2 = document.querySelector("#flexRadioDefault2");
    const radioBtn3 = document.querySelector("#flexRadioDefault3");
    const genderSelect = document.querySelector("#genderSelect");

    radioBtn1.addEventListener("change", () => {
        genderSelect.classList.add("d-none");
    });
    radioBtn2.addEventListener("change", () => {
        genderSelect.classList.add("d-none");
    });
    radioBtn3.addEventListener("change", () => {
        genderSelect.classList.remove("d-none");
    });
}

const checkBox = document.querySelector("#checkbox");
console.log(checkBox);
const btnPost = document.querySelector("#btn-post");
btnPost.addEventListener("click", () => {
    console.log("ffdfd");
});

checkBox.addEventListener("change", () => {
    if (checkBox.checked) {
        btnPost.removeAttribute("disabled");

        // console.log("fddfd");
    } else {
        btnPost.disabled = true;
    }
});