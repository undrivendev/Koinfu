SELECT ex."name"
	,basecurrency
	,countercurrency
FROM tick t
INNER JOIN currencypair cp ON t.currencypairid = cp.id
INNER JOIN exchange ex ON t.exchangeid = ex.id
WHERE t."timestamp" > '2018-01-01T00:00:00'
GROUP BY (
		ex."name"
		,basecurrency
		,countercurrency
		)
ORDER BY 1
