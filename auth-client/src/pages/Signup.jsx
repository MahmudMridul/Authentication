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
import { signUp } from "@/slices/authSlice";
import { Eye, LoaderCircle } from "lucide-react";
import { EyeClosed } from "lucide-react";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useDispatch } from "react-redux";
import { NavLink } from "react-router";

export default function Signup() {
	const dispatch = useDispatch();
	const { toast } = useToast();
	const states = useSelector((store) => store.auth);
	const { loading } = states;

	const [firstName, setFirstName] = useState("");
	const [lastName, setLastName] = useState("");
	const [userName, setUsername] = useState("");
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [showPassword, setShowPassword] = useState(false);
	const [minLength, setMinLength] = useState(true);
	const [upperCase, setUpperCase] = useState(true);
	const [lowerCase, setLowerCase] = useState(true);
	const [number, setNumber] = useState(true);
	const [specialChar, setSpecialChar] = useState(true);

	const [validFirstName, setValidFirstName] = useState(true);
	const [validLastName, setValidLastName] = useState(true);
	const [validUsername, setValidUsername] = useState(true);
	const [validEmail, setValidEmail] = useState(true);

	const validInput =
		firstName.length > 0 &&
		validFirstName &&
		lastName.length > 0 &&
		validLastName &&
		userName.length > 0 &&
		validUsername &&
		email.length > 0 &&
		validEmail &&
		password.length >= 8 &&
		minLength &&
		upperCase &&
		lowerCase &&
		number &&
		specialChar;

	function handleFirstName(e) {
		let v = e.target.value;

		if (v === "") {
			setValidFirstName(true);
			setFirstName(v);
			return;
		}

		if (!isValidName(v)) {
			setValidFirstName(false);
		} else {
			setValidFirstName(true);
		}

		setFirstName(v);
	}

	function handleLastName(e) {
		let v = e.target.value;

		if (v === "") {
			setValidLastName(true);
			setLastName(v);
			return;
		}

		if (!isValidName(v)) {
			setValidLastName(false);
		} else {
			setValidLastName(true);
		}
		setLastName(v);
	}

	function handleUsername(e) {
		let v = e.target.value;

		if (v === "") {
			setValidUsername(true);
			setUsername(v);
			return;
		}

		if (!isValidUsername(v)) {
			setValidUsername(false);
		} else {
			setValidUsername(true);
		}
		setUsername(v);
	}

	function handleEmail(e) {
		let v = e.target.value;

		if (v === "") {
			setValidEmail(true);
			setEmail(v);
			return;
		}

		if (!isValidEmail(v)) {
			setValidEmail(false);
		} else {
			setValidEmail(true);
		}
		setEmail(v);
	}
	function handlePassword(e) {
		let v = e.target.value;

		if (v === "") {
			setMinLength(true);
			setUpperCase(true);
			setLowerCase(true);
			setNumber(true);
			setSpecialChar(true);
			setPassword(v);
			return;
		}

		if (!hasMinLength(v)) {
			setMinLength(false);
		} else {
			setMinLength(true);
		}

		if (!hasUpperCase(v)) {
			setUpperCase(false);
		} else {
			setUpperCase(true);
		}

		if (!hasLowerCase(v)) {
			setLowerCase(false);
		} else {
			setLowerCase(true);
		}

		if (!hasNumber(v)) {
			setNumber(false);
		} else {
			setNumber(true);
		}

		if (!hasSpecialChar(v)) {
			setSpecialChar(false);
		} else {
			setSpecialChar(true);
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
		dispatch(signUp(payload)).then((res) => {
			if (!res.payload.success) {
				toast({
					description: res.payload.message,
					variant: "destructive",
				});
			}
		});
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
				<div
					className={`text-xs text-rose-600 ${
						validFirstName ? "hidden" : "block"
					}`}
				>
					First name should contain alphabets and spaces only
				</div>
				<Input
					type="text"
					placeholder="Last Name"
					value={lastName}
					onChange={handleLastName}
				/>
				<div
					className={`text-xs text-rose-600 ${
						validLastName ? "hidden" : "block"
					}`}
				>
					Last name should contain alphabets and spaces only
				</div>
				<Input
					type="text"
					placeholder="Username"
					value={userName}
					onChange={handleUsername}
				/>
				<div
					className={`text-xs text-rose-600 ${
						validUsername ? "hidden" : "block"
					}`}
				>
					Username should contain alphabets, numbers, special characters and no
					space
				</div>
				<Input
					type="email"
					placeholder="Email"
					value={email}
					onChange={handleEmail}
				/>
				<div
					className={`text-xs text-rose-600 ${validEmail ? "hidden" : "block"}`}
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
					<div className={`${minLength ? "hidden" : "block"}`}>
						Password should contain at least 8 characters
					</div>
					<div className={`${upperCase ? "hidden" : "block"}`}>
						Password should contain at least 1 uppercase letter
					</div>
					<div className={`${lowerCase ? "hidden" : "block"}`}>
						Password should contain at least 1 lowercase letter
					</div>
					<div className={`${number ? "hidden" : "block"}`}>
						Password should contain at least 1 number
					</div>
					<div className={`${specialChar ? "hidden" : "block"}`}>
						Password should contain at least 1 special character
					</div>
				</div>
				<div className="flex justify-between">
					<Button variant="link">
						<NavLink to="/">Sign in</NavLink>
					</Button>
					<Button onClick={handleSignUp} disabled={!validInput}>
						{loading ? (
							<>
								<LoaderCircle className="animate-spin" /> Loading...
							</>
						) : (
							"Sign up"
						)}
					</Button>
				</div>
			</div>
		</div>
	);
}
