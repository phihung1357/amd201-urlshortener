export default function UrlList({ items }) {
    return (
        <div className="card">
            <h2>Created URLs</h2>

            {items.length === 0 ? (
                <p>No URLs yet.</p>
            ) : (
                <table>
                    <thead>
                        <tr>
                            <th>Short Code</th>
                            <th>Original URL</th>
                            <th>Total Clicks</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {items.map((item) => (
                            <tr key={item.id}>
                                <td>{item.shortCode}</td>
                                <td>
                                    <a href={item.originalUrl} target="_blank" rel="noreferrer">
                                        {item.originalUrl}
                                    </a>
                                </td>
                                <td>{item.totalClicks}</td>
                                <td>{item.status}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}