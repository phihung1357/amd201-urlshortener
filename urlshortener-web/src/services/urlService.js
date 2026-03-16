import axios from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:8080",
});

export const createShortUrl = async (payload) => {
    const response = await api.post("/api/Urls", payload);
    return response.data;
};

export const getAllUrls = async () => {
    const response = await api.get("/api/Urls");
    return response.data;
};