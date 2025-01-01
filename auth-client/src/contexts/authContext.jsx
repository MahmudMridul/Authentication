import { createContext, useEffect, useState } from "react";
import PropTypes from "prop-types";
import { useDispatch } from "react-redux";
import { checkAuth } from "@/slices/authSlice";

const AuthContext = createContext(null);

AuthProvider.propTypes = {
	children: PropTypes.node.isRequired,
};

export function AuthProvider({ children }) {
	const dispatch = useDispatch();
	const [isAuthenticated, setIsAuthenticated] = useState(false);

	useEffect(() => {
		dispatch(checkAuth()).then((res) => {
			if (res.payload.success) {
				setIsAuthenticated(true);
			}
		});
	}, []);

	return (
		<AuthContext.Provider value={{ isAuthenticated }}>
			{children}
		</AuthContext.Provider>
	);
}

export default AuthContext;
