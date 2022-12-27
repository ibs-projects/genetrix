options = {
    chart: { height: 339, type: "line", stacked: !1, toolbar: { show: !1 } },
    stroke: { width: [0, 2, 4], curve: "smooth" },
    plotOptions: { bar: { columnWidth: "30%" } },
    colors: ["#5b73e8", "#dfe2e6", "#f1b44c"],
    series: [
        { name: "BGFI", type: "column", data: [23, 11, 22, 27, 13, 22, 37, 21, 44, 22, 30] },
        { name: "UBA", type: "area", data: [44, 55, 41, 67, 22, 43, 21, 41, 56, 27, 43] },
        { name: "BICIG", type: "line", data: [30, 25, 36, 30, 45, 35, 64, 52, 59, 36, 39] }],
    fill: {
        opacity: [.85, .25, 1],
        gradient: { inverseColors: !1, shade: "light", type: "vertical", opacityFrom: .85, opacityTo: .55, stops: [0, 100, 100, 100] }
    },
    labels: ["01/01/2003", "02/01/2003", "03/01/2003", "04/01/2003", "05/01/2003", "06/01/2003", "07/01/2003", "08/01/2003", "09/01/2003", "10/01/2003", "11/01/2003"],
    markers: { size: 0 }, xaxis: { type: "datetime" }, yaxis: { title: { text: "Points" } },
    tooltip: {
        shared: !0, intersect: !1, y: {
            formatter: function (e) { return void 0 !== e ? e.toFixed(0) + " points" : e }
        }
    }, grid: { borderColor: "#f1f1f1" }
};
(chart = new ApexCharts(document.querySelector("#sales-analytics-chart"), options)).render();
