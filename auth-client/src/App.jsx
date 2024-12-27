import { BrowserRouter, Route, Routes } from "react-router";
import Signin from "./components/Signin";
import Signup from "./components/Signup";

export default function App() {
	return (
		<BrowserRouter>
			<Routes>
				<Route path="/" element={<Signin />} />
				<Route path="/signup" element={<Signup />} />
			</Routes>
		</BrowserRouter>
	);
}
