import { Box, Container, Paper } from "@mui/material";
import { FC } from "react";
import { Spacer } from "../../../../presentation/common/Spacer";
import { useSearchState } from "../../hooks/useSearchState";
import { Filters } from "./Filters";
import { Results } from "./Results";
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
      <Container>
        <Spacer sx={{ height: 30 }} />

        <Box sx={{ display: "flex" }}>
          <Box sx={{ width: 250 }}>
            <Filters searchState={searchState} />
          </Box>

          <Spacer sx={{ width: 30 }} />

          <Box sx={{ flexGrow: 1 }}>
            <Results searchState={searchState} />
          </Box>
        </Box>
      </Container>
    </>
  );
};
