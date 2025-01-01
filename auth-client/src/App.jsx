import { BrowserRouter, Route, Routes } from "react-router";
import Signin from "./pages/Signin";
import Signup from "./pages/Signup";
import { Toaster } from "./components/ui/toaster";
import Home from "./pages/Home";
import ProtectedRoute from "./components/custom/ProtectedRoute";

export default function App() {
	return (
		<>
			<Toaster />
			<BrowserRouter>
				<Routes>
					<Route path="/" element={<Signin />} />
					<Route path="/signup" element={<Signup />} />
					<Route
						path="/home"
						element={
							<ProtectedRoute>
								<Home />
							</ProtectedRoute>
						}
					/>
				</Routes>
			</BrowserRouter>
		</>
	);
}
