import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
	hasLowerCase,
	hasMinLength,
	hasNumber,
	hasSpecialChar,
	hasUpperCase,
	isValidEmail,
	isValidName,
	isValidUsername,
} from "@/helpers/functions";
import { useToast } from "@/hooks/use-toast";
import { Eye } from "lucide-react";
import { EyeClosed } from "lucide-react";
// import { signUp } from "@/slices/authSlice";
import { useState } from "react";
// import { useDispatch } from "react-redux";
import { NavLink } from "react-router";

export default function Signup() {
	// const dispatch = useDispatch();
	const { toast } = useToast();

	const [firstName, setFirstName] = useState("");
	const [lastName, setLastName] = useState("");
	const [userName, setUsername] = useState("");
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [invalidUsername, setInvalidUsername] = useState(false);
	const [invalidEmail, setInvalidEmail] = useState(false);
	const [showPassword, setShowPassword] = useState(false);
	const [minLength, setMinLength] = useState(false);
	const [upperCase, setUpperCase] = useState(false);
	const [lowerCase, setLowerCase] = useState(false);
	const [number, setNumber] = useState(false);
	const [specialChar, setSpecialChar] = useState(false);

	function handleFirstName(e) {
		let v = e.target.value;
		if (!isValidName(v)) {
			toast({
				description: "First name should contain alphabets only",
				variant: "destructive",
			});
			return;
		}
		setFirstName(v);
	}

	function handleLastName(e) {
		let v = e.target.value;
		if (!isValidName(v)) {
			toast({
				description: "Last name should contain alphabets only",
				variant: "destructive",
			});
			return;
		}
		setLastName(v);
	}

	function handleUsername(e) {
		let v = e.target.value;

		if (v === "") {
			setInvalidUsername(false);
			setUsername(v);
			return;
		}

		if (!isValidUsername(v)) {
			setInvalidUsername(true);
		} else {
			setInvalidUsername(false);
		}
		setUsername(v);
	}

	function handleEmail(e) {
		let v = e.target.value;

		if (v === "") {
			setInvalidEmail(false);
			setEmail(v);
			return;
		}

		if (!isValidEmail(v)) {
			setInvalidEmail(true);
		} else {
			setInvalidEmail(false);
		}
		setEmail(v);
	}
	function handlePassword(e) {
		let v = e.target.value;

		if (v === "") {
			setMinLength(false);
			setUpperCase(false);
			setLowerCase(false);
			setNumber(false);
			setSpecialChar(false);
			setPassword(v);
			return;
		}

		if (!hasMinLength(v)) {
			setMinLength(true);
		} else {
			setMinLength(false);
		}

		if (!hasUpperCase(v)) {
			setUpperCase(true);
		} else {
			setUpperCase(false);
		}

		if (!hasLowerCase(v)) {
			setLowerCase(true);
		} else {
			setLowerCase(false);
		}

		if (!hasNumber(v)) {
			setNumber(true);
		} else {
			setNumber(false);
		}

		if (!hasSpecialChar(v)) {
			setSpecialChar(true);
		} else {
			setSpecialChar(false);
		}
		setPassword(v);
	}

	function handleSignUp() {
		const payload = {
			userName,
			email,
			password,
			firstName,
			lastName,
		};
		console.log(payload);
		// dispatch(signUp(payload));
	}

	return (
		<div className="container mx-auto h-screen p-5 flex justify-center items-center">
			<div className="w-1/5">
				<Input
					type="text"
					placeholder="First Name"
					value={firstName}
					onChange={handleFirstName}
				/>
				<Input
					type="text"
					placeholder="Last Name"
					value={lastName}
					onChange={handleLastName}
				/>
				<Input
					type="text"
					placeholder="Username"
					value={userName}
					onChange={handleUsername}
				/>
				<div
					className={`text-xs text-rose-600 ${
						invalidUsername ? "block" : "hidden"
					}`}
				>
					Username should contain alphabets, numbers and special characters
				</div>
				<Input
					type="email"
					placeholder="Email"
					value={email}
					onChange={handleEmail}
				/>
				<div
					className={`text-xs text-rose-600 ${
						invalidEmail ? "block" : "hidden"
					}`}
				>
					Email should be in the format of email@example.com
				</div>
				<div className="flex">
					<Input
						type={showPassword ? "text" : "password"}
						placeholder="Password"
						value={password}
						onChange={handlePassword}
					/>
					<Button
						variant="ghost"
						size="sm"
						className="mt-2 ml-2"
						onClick={() => setShowPassword(!showPassword)}
					>
						{showPassword ? <Eye /> : <EyeClosed />}
					</Button>
				</div>
				<div className="text-xs text-rose-600">
					<div className={`${minLength ? "block" : "hidden"}`}>
						Password should contain at least 8 characters
					</div>
					<div className={`${upperCase ? "block" : "hidden"}`}>
						Password should contain at least 1 uppercase letter
					</div>
					<div className={`${lowerCase ? "block" : "hidden"}`}>
						Password should contain at least 1 lowercase letter
					</div>
					<div className={`${number ? "block" : "hidden"}`}>
						Password should contain at least 1 number
					</div>
					<div className={`${specialChar ? "block" : "hidden"}`}>
						Password should contain at least 1 special character
					</div>
				</div>
				<div className="flex justify-between">
					<Button variant="link">
						<NavLink to="/">Sign in</NavLink>
					</Button>
					<Button onClick={handleSignUp}>Sign up</Button>
				</div>
			</div>
		</div>
	);
}
