import { Navigate, useLocation } from "react-router";
import PropTypes from "prop-types";
import { isTokenExpired } from "@/helpers/functions";
// import { useToast } from "@/hooks/use-toast";

ProtectedRoute.propTypes = {
	children: PropTypes.node.isRequired,
};

export default function ProtectedRoute({ children }) {
	// const { toast } = useToast();
	const accessToken = localStorage.getItem("accessToken");
	console.log("accessToken", accessToken);

	const location = useLocation();

	if (isTokenExpired(accessToken)) {
		localStorage.removeItem("accessToken");
	}

	if (!accessToken) {
		// toast({
		// 	description: "",
		// });
		return <Navigate to="/" state={{ from: location }} replace />;
	}
	return children;
}
