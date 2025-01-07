import { getdata, signup, signin, signout, isAuth } from "@/helpers/apis";
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

export const signIn = createAsyncThunk("auth/signin", async (payload) => {
	try {
		const res = await fetch(signin, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
			},
			credentials: "include",
			body: JSON.stringify(payload),
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

export const signOut = createAsyncThunk("auth/signOut", async () => {
	try {
		const res = await fetch(signout, {
			method: "DELETE",
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
				Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
			},
			credentials: "include",
		});

		if (!res.ok) {
			console.error(`https status error ${res.status} ${res.statusText}`);
		}

		const data = await res.json();
		return data;
	} catch (err) {
		console.error("auth/signOut", err);
	}
});

export const checkAuth = createAsyncThunk("auth/checkAuth", async () => {
	try {
		const res = await fetch(isAuth, {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
			},
			credentials: "include",
		});

		if (!res.ok) {
			console.error(`https status error ${res.status} ${res.statusText}`);
		}

		const data = await res.json();
		return data;
	} catch (err) {
		console.error("auth/checkAuth", err);
	}
});

export const getData = createAsyncThunk("auth/getData", async () => {
	try {
		const res = await fetch(getdata, {
			method: "GET",
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
				Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
			},
			credentials: "include",
		});
		if (!res.ok) {
			console.error(`https status error ${res.status} ${res.statusText}`);
		}
		const data = await res.json();
		return data;
	} catch (err) {
		console.error("auth/getData", err);
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
			})

			.addCase(checkAuth.pending, (state) => {
				state.loading = true;
			})
			.addCase(checkAuth.fulfilled, (state, action) => {
				state.loading = false;
				console.log(action.payload);
			})
			.addCase(checkAuth.rejected, (state) => {
				state.loading = false;
			})

			.addCase(getData.pending, (state) => {
				state.loading = true;
			})
			.addCase(getData.fulfilled, (state, action) => {
				state.loading = false;
				console.log(action.payload);
			})
			.addCase(getData.rejected, (state) => {
				state.loading = false;
			})

			.addCase(signOut.pending, (state) => {
				state.loading = true;
			})
			.addCase(signOut.fulfilled, (state, action) => {
				state.loading = false;
				console.log(action.payload);
			})
			.addCase(signOut.rejected, (state) => {
				state.loading = false;
			});
	},
});

export const { set, setLoading } = authSlice.actions;
export default authSlice.reducer;
