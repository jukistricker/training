const nameInput = document.getElementById("name") as HTMLInputElement;
const emailInput = document.getElementById("email") as HTMLInputElement;
const submitBtn = document.getElementById("submit-btn") as HTMLButtonElement;
const formState = {
  isNameValid: false,
  isEmailValid: false,
};
const nameError = document.getElementById("name-error") as HTMLDivElement;
const emailError = document.getElementById("email-error") as HTMLDivElement;
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const form = document.getElementById("form") as HTMLFormElement;
const successMsg = document.getElementById("success-msg") as HTMLDivElement;

nameInput.addEventListener("input", () => {
  const nameValue = nameInput.value.trim();
  if (nameValue === "") {
    formState.isNameValid = false;
    nameError.textContent = "Name is required."; 
    nameInput.classList.add("invalid");
  } else if (nameValue.length < 3) {
    formState.isNameValid = false;
    nameError.textContent = "Name must be at least 3 characters long.";
    nameInput.classList.add("invalid");
  } else {
    formState.isNameValid = true;
    nameError.textContent = "";
    nameInput.classList.remove("invalid");
  }
  updateSubmitButton();
});

emailInput.addEventListener("input", () => {
  const emailValue = emailInput.value.trim();
  if (!emailRegex.test(emailValue)) {
    formState.isEmailValid = false;
    emailError.textContent = "Please enter a valid email address.";
    emailInput.classList.add("invalid");
  } else {
    formState.isEmailValid = true;
    emailError.textContent = "";
    emailInput.classList.remove("invalid");
  }
  updateSubmitButton();
});

const updateSubmitButton = () => {
  if (formState.isNameValid && formState.isEmailValid) {
    submitBtn.disabled = false;
  } else {
    submitBtn.disabled = true;
  }
};

form.addEventListener("submit", (event: SubmitEvent) => {
  event.preventDefault();
  handleSuccess();
});

const handleSuccess = () => {
  successMsg.classList.add("show");

  form.reset();

  formState.isNameValid = false;
  formState.isEmailValid = false;
  updateSubmitButton();

  nameInput.classList.remove("invalid");
  emailInput.classList.remove("invalid");
  nameError.textContent = "";
  emailError.textContent = "";

  setTimeout(() => {
    successMsg.classList.remove("show");
  }, 3000);
};
