import { NavLink, useNavigate } from "react-router";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useState } from "react";
import { isValidEmail, isValidUsername } from "@/helpers/functions";
import { useDispatch } from "react-redux";
import { signIn } from "@/slices/authSlice";
import { useToast } from "@/hooks/use-toast";

export default function Signin() {
	const dispatch = useDispatch();
	const { toast } = useToast();
	const navigate = useNavigate();

	const [nameOrEmail, setNameOrEmail] = useState("");
	const [password, setPassword] = useState("");

	const validInput =
		nameOrEmail.length > 0 &&
		password.length > 0 &&
		(isValidUsername(nameOrEmail) || isValidEmail(nameOrEmail));

	function handleNameOrEmail(e) {
		let v = e.target.value;
		setNameOrEmail(v);
	}

	function handlePassword(e) {
		let v = e.target.value;
		setPassword(v);
	}

	function handleSignin() {
		const payload = {
			nameOrEmail,
			password,
		};
		dispatch(signIn(payload)).then((res) => {
			let variant = res.payload.success ? "default" : "destructive";
			toast({
				description: res.payload.message,
				variant: variant,
			});

			if (res.payload.success) {
				navigate("/home");
			} else {
				navigate("/");
			}
		});
	}

	return (
		<div className="container mx-auto h-screen p-5 flex justify-center items-center">
			<div className="w-1/5">
				<Input
					type="text"
					placeholder="Username or email"
					value={nameOrEmail}
					onChange={handleNameOrEmail}
				/>
				<Input
					type="password"
					placeholder="Password"
					value={password}
					onChange={handlePassword}
				/>
				<div className="flex justify-between">
					<Button variant="link">
						<NavLink to="/signup">Sign up</NavLink>
					</Button>
					<Button onClick={handleSignin} disabled={!validInput}>
						Sign in
					</Button>
				</div>
			</div>
		</div>
	);
}
