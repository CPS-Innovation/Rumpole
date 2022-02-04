import { useEffect, useState } from "react";
import { SearchResults } from "./SearchResults";

export const Search: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [results, setResults] = useState<SearchResults | undefined>();

  useEffect(() => {
    (async () => {
      if (!searchTerm) return;

      const searchResponse = await fetch(
        process.env.REACT_APP_SEARCH_INDEX! + searchTerm
      );

      const results = (await searchResponse.json()) as SearchResults;

      setResults(results);
    })();
  }, [searchTerm]);

  return (
    <>
      <input onChange={(ev) => setSearchTerm(ev.target.value)} />

      <br />
      <ul>
        {results &&
          results.value.map((item) => (
            <li>
              <div>
                Doc: {item.documentId}, page:{item.pageIndex}, line:{" "}
                {item.lineIndex}
                <br />
                {item.text}
              </div>
            </li>
          ))}
      </ul>
    </>
  );
};
