import { NavLink, useNavigate } from "react-router";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useState } from "react";
import { isValidEmail, isValidUsername } from "@/helpers/functions";
import { useDispatch, useSelector } from "react-redux";
import { signIn } from "@/slices/authSlice";
import { useToast } from "@/hooks/use-toast";
import { Eye, EyeClosed, LoaderCircle } from "lucide-react";

export default function Signin() {
	const dispatch = useDispatch();
	const { toast } = useToast();
	const navigate = useNavigate();

	const states = useSelector((store) => store.auth);
	const { loading } = states;

	const [nameOrEmail, setNameOrEmail] = useState("");
	const [password, setPassword] = useState("");
	const [showPassword, setShowPassword] = useState(false);

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
				// setIsAuthenticated(true);
				console.log("Signin successful");
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
				<div className="flex justify-between">
					<Button variant="link">
						<NavLink to="/signup">Sign up</NavLink>
					</Button>
					<Button onClick={handleSignin} disabled={!validInput}>
						{loading ? (
							<>
								<LoaderCircle className="animate-spin" /> Loading...
							</>
						) : (
							"Sign in"
						)}
					</Button>
				</div>
			</div>
		</div>
	);
}
