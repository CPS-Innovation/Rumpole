import { Button, Checkbox, FormControlLabel, FormGroup } from "@mui/material";
import { FC } from "react";
import { Spacer } from "../../../../presentation/common/Spacer";
import { ArrayFiltertype, FilterDetails } from "../../hooks/useSearchState";
import { Header } from "./Header";

type FilterProps = {
  title: string;
  filterDetails: FilterDetails<ArrayFiltertype>;
};

export const FilterMany: FC<FilterProps> = ({ title, filterDetails }) => {
  return (
    <>
      <Header>{title}</Header>
      <Spacer sx={{ height: 10 }} />
      <Button
        variant="text"
        color="primary"
        sx={{
          display: "inline-block",
          padding: 0,
          minHeight: 0,
          minWidth: 0,
        }}
      >
        All
      </Button>
      <FormGroup>
        {Object.keys(filterDetails).map((key) => {
          const item = filterDetails[key];
          const label = `${item.name} (${item.count})`;
          const isChecked = item.selected === undefined;
          return <FormControlLabel control={<Checkbox />} label={label} />;
        })}
      </FormGroup>

      <Spacer sx={{ height: 20 }} />
    </>
  );
};
