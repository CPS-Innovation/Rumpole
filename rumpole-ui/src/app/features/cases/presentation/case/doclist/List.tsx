import {
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { FC } from "react";
import { Spacer } from "../../../../../common/presentation/components/Spacer";
import {
  commonDateTimeFormats,
  formatISODate,
} from "../../../../../common/utils/dates";

type ListProps = {
  heading: string;
  documents: { id: number; name: string; createdDate: string }[];
};

export const List: FC<ListProps> = ({ heading, documents, children }) => {
  return (
    <>
      <Spacer sx={{ height: 30 }} />
      <Paper sx={{ margin: "10px", padding: "20px" }} elevation={2}>
        <Typography variant="h5" component="h3">
          {heading}
        </Typography>
        <Spacer sx={{ height: 15 }} />

        {children && <Box>{children}</Box>}

        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Document</TableCell>
              <TableCell>Created Date</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {documents.map(({ id, name, createdDate }) => (
              <TableRow key={id}>
                <TableCell sx={{ borderBottom: "none" }}>
                  <Typography variant="body1">{name}</Typography>
                </TableCell>
                <TableCell sx={{ borderBottom: "none" }}>
                  <Typography variant="body1">
                    {formatISODate(
                      createdDate,
                      commonDateTimeFormats.shortDate
                    )}
                  </Typography>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </>
  );
};
