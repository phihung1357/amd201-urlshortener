export default function UrlResultCard({ result }) {
    if (!result) return null;

    const handleCopy = async () => {
        await navigator.clipboard.writeText(result.shortLink);
        alert("Copied short link!");
    };

    return (
        <div className="card">
            <h2>Latest Result</h2>
            <p><strong>Original URL:</strong> {result.originalUrl}</p>
            <p><strong>Short Code:</strong> {result.shortCode}</p>
            <p><strong>Status:</strong> {result.status}</p>
            <p>
                <strong>Short Link:</strong>{" "}
                <a href={result.shortLink} target="_blank" rel="noreferrer">
                    {result.shortLink}
                </a>
            </p>
            <button onClick={handleCopy}>Copy Short Link</button>
        </div>
    );
}