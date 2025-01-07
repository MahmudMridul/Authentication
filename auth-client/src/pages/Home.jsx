import { Button } from "@/components/ui/button";
import { getData, signOut } from "@/slices/authSlice";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router";

export default function Home() {
	const dispatch = useDispatch();
	const navigate = useNavigate();

	const userName = localStorage.getItem("userName");
	const email = localStorage.getItem("email");

	const [data, setData] = useState([]);

	useEffect(() => {
		dispatch(getData()).then((res) => {
			if (res.payload.data) {
				setData(res.payload.data);
			} else {
				console.log("No data found");
			}
		});
	}, []);

	function handleSignout() {
		dispatch(signOut()).then((res) => {
			if (res.payload.success) {
				localStorage.removeItem("accessToken");
				localStorage.removeItem("userName");
				localStorage.removeItem("email");
				navigate("/");
			}
		});
	}

	return (
		<div className="container mx-auto p-5 flex">
			<div className="mx-10">
				<h1>Home</h1>
				<div>{userName}</div>
				<div>{email}</div>

				{data.map((item, index) => {
					return <div key={index}>{item}</div>;
				})}
			</div>
			<div className="mx-10">
				<Button onClick={handleSignout}>Singout</Button>
			</div>
		</div>
	);
}
