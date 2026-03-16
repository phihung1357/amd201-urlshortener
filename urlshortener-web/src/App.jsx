import { useEffect, useState } from "react";
import UrlForm from "./components/UrlForm";
import UrlResultCard from "./components/UrlResultCard";
import UrlList from "./components/UrlList";
import { createShortUrl, getAllUrls } from "./services/urlService";
import "./index.css";

export default function App() {
    const [result, setResult] = useState(null);
    const [urls, setUrls] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const loadUrls = async () => {
        try {
            const data = await getAllUrls();
            setUrls(data);
        } catch (err) {
            console.error(err);
        }
    };

    useEffect(() => {
        loadUrls();
    }, []);

    const handleCreate = async (payload) => {
        try {
            setLoading(true);
            setError("");

            const data = await createShortUrl(payload);
            setResult(data);
            await loadUrls();
        } catch (err) {
            console.error(err);
            setError(err?.response?.data?.message || "Create failed.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container">
            <h1>URL Shortener</h1>
            <p>Simple web UI for AMD201 coursework</p>

            {error && <div className="error-box">{error}</div>}

            <UrlForm onCreate={handleCreate} loading={loading} />
            <UrlResultCard result={result} />
            <UrlList items={urls} />
        </div>
    );
}