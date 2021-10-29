import { Search } from "@mui/icons-material";
import { Button, Container, Paper } from "@mui/material";
import { FC, useState } from "react";
import { isUrnValid } from "../../logic/isUrnValid";
import { UrnField } from "./UrnField";

export const UrnSearch: FC = () => {
  const [urn, setUrn] = useState("");
  const isValid = isUrnValid(urn);

  const handleChange = (val: string) => setUrn(val.toUpperCase());

  const handleSubmit = () => {
    isValid && console.log(urn);
  };

  return (
    <>
      <Paper>
        <Container
          sx={{
            height: 60,
            display: "flex",
            alignItems: "center",
          }}
          maxWidth="md"
        >
          <Search fontSize="large" />

          <UrnField
            value={urn}
            onChange={handleChange}
            onSubmit={handleSubmit}
          />

          <Button
            variant="contained"
            color="secondary"
            onClick={handleSubmit}
            disabled={!isValid}
          >
            Search
          </Button>
        </Container>
      </Paper>
    </>
  );
};
