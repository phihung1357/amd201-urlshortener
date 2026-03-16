import { useState } from "react";

export default function UrlForm({ onCreate, loading }) {
    const [originalUrl, setOriginalUrl] = useState("");
    const [customAlias, setCustomAlias] = useState("");
    const [expiresAt, setExpiresAt] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();

        await onCreate({
            originalUrl,
            customAlias: customAlias || null,
            expiresAt: expiresAt || null,
        });
    };

    return (
        <form onSubmit={handleSubmit} className="card">
            <h2>Create Short URL</h2>

            <label>Original URL</label>
            <input
                type="text"
                placeholder="https://example.com"
                value={originalUrl}
                onChange={(e) => setOriginalUrl(e.target.value)}
                required
            />

            <label>Custom Alias</label>
            <input
                type="text"
                placeholder="my-link"
                value={customAlias}
                onChange={(e) => setCustomAlias(e.target.value)}
            />

            <label>Expires At</label>
            <input
                type="datetime-local"
                value={expiresAt}
                onChange={(e) => setExpiresAt(e.target.value)}
            />

            <button type="submit" disabled={loading}>
                {loading ? "Creating..." : "Create Short URL"}
            </button>
        </form>
    );
}