import { useSelector } from "react-redux";
import { Navigate, useLocation } from "react-router";
import PropTypes from "prop-types";

ProtectedRoute.propTypes = {
	children: PropTypes.node.isRequired,
};

export default function ProtectedRoute({ children }) {
	const states = useSelector((store) => store.auth);
	console.log("Full auth state:", states);
	const { isAuthenticated } = states;
	console.log("isAuth", isAuthenticated);

	const location = useLocation();

	if (!isAuthenticated) {
		return <Navigate to="/" state={{ from: location }} replace />;
	}
	return children;
}
