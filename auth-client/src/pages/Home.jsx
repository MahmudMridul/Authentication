import { Button } from "@/components/ui/button";
import { getData, signOut } from "@/slices/authSlice";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router";

export default function Home() {
	const dispatch = useDispatch();
	const navigate = useNavigate();

	const states = useSelector((state) => state.auth);
	const { signedInUser } = states;

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
				navigate("/");
			}
		});
	}

	return (
		<div className="container mx-auto p-5 flex">
			<div className="mx-10">
				<h1>Home</h1>
				<div>{signedInUser.userName}</div>
				<div>{signedInUser.firstName}</div>
				<div>{signedInUser.lastName}</div>
				<div>{signedInUser.email}</div>

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
