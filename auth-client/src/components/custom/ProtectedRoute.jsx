import AuthContext from "@/contexts/authContext";
import { useContext, useEffect } from "react";
import { useNavigate } from "react-router";

export default function ProtectedRoute({ children }) {
	const navigate = useNavigate();
	const { isAuthenticated } = useContext(AuthContext);
	console.log(isAuthenticated);

	useEffect(() => {
		if (!isAuthenticated) {
			navigate("/", { replace: true });
		}
	}, []);
	return children;
}
