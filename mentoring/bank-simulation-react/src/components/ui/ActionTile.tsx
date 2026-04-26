import { Link } from "react-router-dom";

interface ActionTileProps {
  to: string;
  icon: string;
  label: string;
  description: string;
  theme: string;
  disabled?: boolean;
}

export const ActionTile = ({ to, icon, label, description, theme, disabled }: ActionTileProps) => {
  const content = (
    <div className={`card h-100 shadow-sm border-0 action-card ${disabled ? "bg-light opacity-50" : ""}`}>
      <div className="card-body text-center p-4">
        <div className={`mb-3 ${theme} ${disabled ? "text-secondary" : ""}`}>
          <i className={`bi ${icon} display-5`}></i>
        </div>
        <h5 className="fw-bold mb-1 card-title-text">{label}</h5>
        <p className="small mb-0 card-description-text">{description}</p>
      </div>
    </div>
  );
  
  if (disabled) {
    return <div className="col-md-4" style={{ cursor: "not-allowed" }}>{content}</div>;
  }

  return (
    <div className="col-md-4">
      <Link to={to} className="text-decoration-none">
        {content}
      </Link>
    </div>
  );
};