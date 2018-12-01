--check if some ticker has stopped
SELECT
    max(a.TIMESTAMP),
    a.exchange,
    a.symbol
FROM (
    SELECT
        tick. "timestamp",
        exchange.NAME exchange,
        CONCAT(currencypair.basecurrency, '-', currencypair.countercurrency) symbol
    FROM
        tick
    INNER JOIN currencypair ON currencypairid = currencypair.id
    INNER JOIN exchange ON exchangeid = exchange.id) a
GROUP BY
    a.exchange,
    a.symbol
