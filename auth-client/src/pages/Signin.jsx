import { NavLink } from "react-router";
import { Button } from "./ui/button";
import { Input } from "./ui/input";

export default function Signin() {
	function handleSignin() {}
	return (
		<div className="container mx-auto h-screen p-5 flex justify-center items-center">
			<div className="w-1/5">
				<Input type="text" placeholder="Username" />
				<Input type="password" placeholder="Password" />
				<div className="flex justify-between">
					<Button variant="link">
						<NavLink to="/signup">Sign up</NavLink>
					</Button>
					<Button onClick={handleSignin}>Sign in</Button>
				</div>
			</div>
		</div>
	);
}
