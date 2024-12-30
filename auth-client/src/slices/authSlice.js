import { signup } from "@/helpers/apis";
import { signin } from "@/helpers/apis";
import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

const initialState = {
	loading: false,
};

export const signUp = createAsyncThunk("auth/signup", async (payload) => {
	try {
		const res = await fetch(signup, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
			},
			body: JSON.stringify(payload),
		});

		if (!res.ok) {
			console.error(`https status error ${res.status} ${res.statusText}`);
		}

		const data = await res.json();
		return data;
	} catch (err) {
		console.error("auth/signUp", err);
	}
});

export const signIn = createAsyncThunk("auth/signin", async ({ payload }) => {
	try {
		const res = await fetch(signin, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
			},
			body: JSON.stringify({ payload }),
		});

		if (!res.ok) {
			console.error(`https status error ${res.status} ${res.statusText}`);
		}

		const data = await res.json();
		return data;
	} catch (err) {
		console.error("auth/signIn", err);
	}
});

export const authSlice = createSlice({
	name: "auth",
	initialState,
	reducers: {
		set: {
			prepare(key, value) {
				return { key, value };
			},
			reducer(action, state) {
				const { key, value } = action.payload;
				state[key] = value;
			},
		},
		setLoading(state, action) {
			state.loading = action.payload;
		},
	},
	extraReducers: (builder) => {
		builder
			.addCase(signUp.pending, (state) => {
				state.loading = true;
			})
			.addCase(signUp.fulfilled, (state, action) => {
				state.loading = false;
				console.log(action.payload);
			})
			.addCase(signUp.rejected, (state) => {
				state.loading = false;
			})

			.addCase(signIn.pending, (state) => {
				state.loading = true;
			})
			.addCase(signIn.fulfilled, (state) => {
				state.loading = false;
			})
			.addCase(signIn.rejected, (state) => {
				state.loading = false;
			});
	},
});

export const { set, setLoading } = authSlice.actions;
export default authSlice.reducer;
