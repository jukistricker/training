import React, { useEffect, useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend,
  AreaChart, Area, PieChart, Pie, Cell
} from 'recharts';
import { RootState, AppDispatch } from '../../../store/store';
import { fetchTransactionHistory } from '../../../store/thunks/transactionThunk';
import { TransactionType } from '../../../types/transaction';
import toast from 'react-hot-toast';

// --- DATA GIẢ LẬP ---
// 1. Phân bổ tài sản (Dạng tròn - Pie Chart)
const assetData = [
  { name: 'Checking Acc', value: 70 },
  { name: 'Savings Acc', value: 20 },
  { name: 'Investments', value: 10 },
];
const ASSET_COLORS = ['#0d6efd', '#198754', '#6610f2'];

// 2. Xu hướng tiết kiệm (Dạng vùng - Area Chart)
const savingsTrendData = [
  { month: 'Jan', savings: 1000 },
  { month: 'Feb', savings: 1200 },
  { month: 'Mar', savings: 900 },
  { month: 'Apr', savings: 1500 },
  { month: 'May', savings: 1800 },
  { month: 'Jun', savings: 2100 },
];
const exportToCSV = (data: any[], filename: string) => {
  if (!data || data.length === 0) {
    toast.error("No data available to export!");
    return;
  }

  // 1. Định nghĩa tiêu đề cột
  const headers = ["ID", "Date", "Type", "Amount ($)", "Description"];
  
  // 2. Chuyển đổi dữ liệu thành các dòng CSV
  const csvRows = data.map(t => [
    t.id,
    new Date(t.created_at).toLocaleString("en-US"),
    t.transaction_type === 0 ? "Deposit" : t.transaction_type === 1 ? "Withdraw" : "Transfer",
    t.amount,
    `"${t.description?.replace(/"/g, '""')}"` // Handle commas in description
  ].join(','));

  // 3. Ghép tiêu đề và nội dung
  const csvContent = [headers.join(','), ...csvRows].join('\n');

  // 4. Tạo Blob và tải về
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.setAttribute("href", url);
  link.setAttribute("download", `${filename}_${new Date().getTime()}.csv`);
  link.style.visibility = 'hidden';
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const DashboardPage: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();

  // 1. Lấy State thật từ Redux
  const { items = [] } = useSelector((state: RootState) => state.transactions || {});
  const { user } = useSelector((state: RootState) => state.account || {});

  // 2. Gọi API để lấy dữ liệu (Chỉ lấy trang 1)
  useEffect(() => {
    if (user?.account_number) {
      dispatch(
        fetchTransactionHistory({
          account_number: user.account_number,
          page: 1, 
        })
      );
    }
  }, [dispatch, user?.account_number]);

  // 3. Logic xử lý dữ liệu thật (Biểu đồ Cash Flow)
  const cashFlowData = useMemo(() => {
    if (!items.length) return [];
    const dailyMap: { [key: string]: { day: string; income: number; expense: number } } = {};

    items.forEach((t) => {
      const dateStr = new Date(t.created_at).toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
      if (!dailyMap[dateStr]) dailyMap[dateStr] = { day: dateStr, income: 0, expense: 0 };
      if (t.transaction_type === TransactionType.Deposit) dailyMap[dateStr].income += t.amount;
      else dailyMap[dateStr].expense += t.amount;
    });

    return Object.values(dailyMap).reverse().slice(-7);
  }, [items]);

  return (
    <div className="container-fluid py-4">
      {/* Header section */}
      <div className="d-flex justify-content-between align-items-center mb-4 pb-2 border-bottom">
        <div>
          <h2 className="fw-bold mb-0">Financial Command Center</h2>
          <p className="text-muted mb-0">Welcome back, {user?.owner_name} | Account: {user?.account_number}</p>
        </div>
        <button 
        className="btn btn-outline-primary btn-sm rounded-pill shadow-sm"
        onClick={() => exportToCSV(items, "transaction_history")}
        disabled={items.length === 0}
      >
        <i className="bi bi-download me-1"></i> 
        {items.length === 0 ? "No Data" : "Export Report (CSV)"}
      </button>
      </div>

      {/* Quick Stats Grid */}
      <div className="row g-3 mb-4">
        <div className="col-md-3">
          <div className="card border-0 bg-primary text-white shadow-sm p-3 h-100">
            <small className="opacity-75">Available Balance</small>
            <h2 className="fw-bold mb-0">${user?.balance.toLocaleString()}</h2>
            <small className="text-white-50"><i className="bi bi-arrow-up me-1"></i>0.5% vs yesterday</small>
          </div>
        </div>
        <div className="col-md-3">
          <div className="card border-0 shadow-sm p-3 h-100">
            <small className="text-muted">Total Debt</small>
            <h2 className="fw-bold mb-0 text-danger">$0</h2>
            <small className="text-success"><i className="bi bi-check-circle me-1"></i>No loans</small>
          </div>
        </div>
        <div className="col-md-3">
          <div className="card border-0 shadow-sm p-3 h-100">
            <small className="text-muted">Investment Value</small>
            <h2 className="fw-bold mb-0 text-success">$2,500</h2>
            <small className="text-danger"><i className="bi bi-arrow-down me-1"></i>1.2% (Simulated)</small>
          </div>
        </div>
        <div className="col-md-3">
          <div className="card border-0 shadow-sm p-3 h-100">
            <small className="text-muted">Savings Goal</small>
            <h2 className="fw-bold mb-0 text-info">$10,000</h2>
            <div className="progress mt-1" style={{height: '6px'}}>
              <div className="progress-bar bg-info" role="progressbar" style={{width: '25%'}}></div>
            </div>
            <small className="text-muted small">25% achieved (Simulated)</small>
          </div>
        </div>
      </div>

      {/* Main Charts Row */}
      <div className="row g-4 mb-4">
        {/* Row 1: Biểu đồ Cash Flow (Dữ liệu thật) */}
        <div className="col-lg-8 col-md-12">
          <div className="card border-0 shadow-sm p-4 h-100">
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h5 className="fw-bold mb-0">Recent Activity (Cash Flow)</h5>
              <span className="badge bg-success-subtle text-success rounded-pill">Real Data</span>
            </div>
            <div style={{ width: '100%', height: 350 }}>
              {items.length > 0 ? (
                <ResponsiveContainer>
                  <BarChart data={cashFlowData}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} opacity={0.3} />
                    <XAxis dataKey="day" tick={{ fontSize: 12 }} />
                    <YAxis tick={{ fontSize: 12 }} unit="$" />
                    <Tooltip 
                      cursor={{ fill: '#353535' }}
                      contentStyle={{ borderRadius: '10px', border: 'none', boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}
                    />
                    <Legend />
                    <Bar name="Income" dataKey="income" fill="#198754" radius={[4, 4, 0, 0]} />
                    <Bar name="Expense" dataKey="expense" fill="#dc3545" radius={[4, 4, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              ) : (
                <div className="d-flex h-100 align-items-center justify-content-center text-muted border border-dashed rounded">
                  No transaction data to display.
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Row 1: Biểu đồ AreaChart - Xu hướng (Dữ liệu giả lập) */}
        <div className="col-lg-4 col-md-12">
          <div className="card border-0 shadow-sm p-4 h-100">
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h5 className="fw-bold mb-0">Savings Trend</h5>
              <span className="badge bg-warning-subtle text-warning rounded-pill">Simulated</span>
            </div>
            <div style={{ width: '100%', height: 350 }}>
              <ResponsiveContainer>
                <AreaChart data={savingsTrendData}>
                  <defs>
                    <linearGradient id="colorSavings" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#17a2b8" stopOpacity={0.8}/>
                      <stop offset="95%" stopColor="#17a2b8" stopOpacity={0}/>
                    </linearGradient>
                  </defs>
                  <XAxis dataKey="month" tick={{fontSize: 12}} />
                  <YAxis tick={{fontSize: 12}} />
                  <CartesianGrid strokeDasharray="3 3" opacity={0.3} />
                  <Tooltip />
                  <Area name="Savings" type="monotone" dataKey="savings" stroke="#17a2b8" fillOpacity={1} fill="url(#colorSavings)" strokeWidth={2}/>
                </AreaChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>
      </div>

      {/* Row 2: Biểu đồ PieChart (Dữ liệu giả lập) */}
      <div className="row g-4">
        <div className="col-lg-4 col-md-6">
          <div className="card border-0 shadow-sm p-4 h-100">
            <div className="d-flex justify-content-between align-items-center mb-4">
              <h5 className="fw-bold mb-0">Asset Allocation</h5>
              <span className="badge bg-warning-subtle text-warning rounded-pill">Simulated</span>
            </div>
            <div style={{ width: '100%', height: 250 }}>
              <ResponsiveContainer>
                <PieChart>
                  <Pie
                    data={assetData}
                    cx="50%" cy="50%"
                    innerRadius={60} outerRadius={80}
                    fill="#8884d8" paddingAngle={5}
                    dataKey="value" nameKey="name"
                    label
                  >
                    {assetData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={ASSET_COLORS[index % ASSET_COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip />
                  <Legend verticalAlign="bottom" />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Mẹo tài chính giả lập */}
        <div className="col-lg-8 col-md-6">
          <div className="card border-0 bg-success-subtle text-success p-4 h-100 shadow-sm">
            <h5 className="fw-bold mb-3"><i className="bi bi-lightbulb-fill me-2"></i>Financial Tip of the Week</h5>
            <p className="mb-2"><strong>Diversify your portfolio:</strong> "Don't put all your eggs in one basket." (Giả lập)</p>
            <p className="mb-3 small">Based on your simulated asset allocation, you are heavily invested in Checking Accounts. Consdering moving some funds to higher-yield savings or diversified stock index funds.</p>
            <button className="btn btn-success btn-sm w-fit rounded-pill">Review Allocation</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;