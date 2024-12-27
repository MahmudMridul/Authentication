import { apis } from "@/helpers/apis";
import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";

const initialState = {
	loading: false,
};

export const signUp = createAsyncThunk("auth/signin", async (payload) => {
	try {
		console.log(payload);
		const url = apis.signUp;
		const res = fetch(url, {
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

		const text = await res.text();
		if (!text) {
			console.error("Empty response");
		}

		try {
			const data = JSON.parse(text);
			return data;
		} catch (parseError) {
			console.error("Failed to parse JSON", text, parseError);
		}
	} catch (err) {
		console.error("auth/signUp", err);
	}
});

export const signIn = createAsyncThunk("auth/signin", async ({ payload }) => {
	try {
		const url = apis.signIn;
		const res = fetch(url, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
			},
			credentials: "include",
			body: JSON.stringify({ payload }),
		});

		if (!res.ok) {
			console.error(`https status error ${res.status} ${res.statusText}`);
		}

		const text = await res.text();
		if (!text) {
			console.error("Empty response");
		}

		try {
			const data = JSON.parse(text);
			return data;
		} catch (parseError) {
			console.error("Failed to parse JSON", text, parseError);
		}
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
			.addCase(signUp.fulfilled, (state) => {
				state.loading = false;
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

export const { set } = authSlice.actions;
export default authSlice.reducer;
