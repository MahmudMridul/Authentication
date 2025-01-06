import { getData } from "@/slices/authSlice";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";

export default function Home() {
	const dispatch = useDispatch();
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

	return (
		<div>
			<h1>Home</h1>
			<div>Username</div>
			<div>First Name</div>
			<div>Last Name</div>
			<div>Email</div>

			{data.map((item, index) => {
				return <div key={index}>{item}</div>;
			})}
		</div>
	);
}
