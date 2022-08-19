import { ScaledPosition } from "../../../../../../react-pdf-highlighter";

// todo: move into own files
export interface NewRedaction {
  position: ScaledPosition;
}

export interface IRedaction extends NewRedaction {
  id: string;
}
