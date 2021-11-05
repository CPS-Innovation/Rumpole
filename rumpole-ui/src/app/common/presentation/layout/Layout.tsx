import AppBar from "./AppBar";

export const Layout: React.FC = ({ children }) => {
  return (
    <>
      <AppBar />
      {children}
    </>
  );
};
