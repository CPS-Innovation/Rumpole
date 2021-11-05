import { Box, Container, Paper } from "@mui/material";
import { FC } from "react";
import { Spacer } from "../../../../common/presentation/components/Spacer";
import { useSearchState } from "../../hooks/useSearchState";
import { Filters } from "./Filters";
import { Results } from "./Results";
import { ResultsTitle } from "./ResultsTitle";
import { SearchBar } from "./SearchBar";

export const path = "/search/";

export const Page: FC = () => {
  const searchState = useSearchState();

  return (
    <>
      <Paper elevation={2}>
        <Container
          sx={{
            height: 70,
            display: "flex",
            alignItems: "center",
          }}
        >
          <SearchBar searchState={searchState} />
        </Container>
      </Paper>

      {searchState.loadingStatus !== "idle" && (
        <Container>
          <Spacer sx={{ height: 30 }} />

          <Box sx={{ display: "flex" }}>
            <Box sx={{ width: 250 }}>
              <Filters searchState={searchState} />
            </Box>

            <Spacer sx={{ width: 30 }} />

            <Box sx={{ flexGrow: 1 }}>
              <ResultsTitle searchState={searchState} />
              <Results searchState={searchState} />
            </Box>
          </Box>
        </Container>
      )}
    </>
  );
};
