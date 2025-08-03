import React, { useEffect, useRef } from 'react';
import {
    ReponsiveContainer,
    LineChart,
    XAxis,
    YAxis,
    Tooltip,
    Legend
} from 'recharts'

/*

props.data: array of {timestamp, string, NEURO: number, PLASMA: number, BIO: number}

*/

export default function LiveChart({ data }) {
    return (
        <div style={{ width: '100%', height: 350 }}>
            <ResponsiveContainer>
                <LineChart data={data}>
                    <XAxis
                        dataKey="timestamp"
                        tickFormatter={t => new Date(t).toLocaleTimeString()}
                    />
                    <YAxis />
                    <Tooltip labelFormatter={t => new Date(t).toLocaleTimeString()} />
                    <Legend verticalAlign="top" />
                    <Line type="monotone" dataKey="NEURO" stroke="#1976d2" dot={false} />
                    <Line type="monotone" dataKey="PLASMA" stroke="#dc004e" dot={false} />
                    <Line type="monotone" dataKey="BIO" stroke="#388e3c" dot={false} />
                </LineChart>
            </ResponsiveContainer>
        </div>
    );
}