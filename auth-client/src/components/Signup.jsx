import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
// import { signUp } from "@/slices/authSlice";
import { useState } from "react";
// import { useDispatch } from "react-redux";
import { NavLink } from "react-router";

export default function Signup() {
	// const dispatch = useDispatch();

	const [firstName, setFirstName] = useState("");
	const [lastName, setLastName] = useState("");
	const [userName, setUsername] = useState("");
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	function handleFirstName(e) {
		let v = e.target.value;
		setFirstName(v);
	}

	function handleLastName(e) {
		let v = e.target.value;
		setLastName(v);
	}

	function handleUsername(e) {
		let v = e.target.value;
		setUsername(v);
	}
	function handleEmail(e) {
		let v = e.target.value;
		setEmail(v);
	}
	function handlePassword(e) {
		let v = e.target.value;
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
				<Input
					type="email"
					placeholder="Email"
					value={email}
					onChange={handleEmail}
				/>
				<Input
					type="password"
					placeholder="Password"
					value={password}
					onChange={handlePassword}
				/>
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
