export function isValidName(name) {
	// Check if the name contains only alphabets
	const hasAlphabets = /^[A-Za-z]+$/.test(name);
	return hasAlphabets;
}

export function isValidUsername(username) {
	const hasAlphabets = /[A-Za-z]/.test(username); // Check for alphabets
	const hasNumbers = /[0-9]/.test(username); // Check for numbers
	const hasSpecialChars = /[^A-Za-z0-9]/.test(username); // Check for special characters
	return hasAlphabets && hasNumbers && hasSpecialChars;
}

export function isValidEmail(email) {
	// Regular expression to validate email
	const validEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
	return validEmail.test(email);
}

export function isValidPassword(password) {
	return (
		hasMinLength(password) &&
		hasUpperCase(password) &&
		hasLowerCase(password) &&
		hasNumber(password) &&
		hasSpecialChar(password)
	);
}
export function hasMinLength(str, length = 8) {
	return str.length >= length;
}

export function hasUpperCase(str) {
	return /[A-Z]/.test(str);
}

export function hasLowerCase(str) {
	return /[a-z]/.test(str);
}

export function hasNumber(str) {
	return /[0-9]/.test(str);
}

export function hasSpecialChar(str) {
	return /[^A-Za-z0-9]/.test(str);
}
