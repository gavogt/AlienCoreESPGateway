import React, { useEffect, useState } from 'react';
import LiveChart from './LiveChart.jsx';
import TelemetryTable from './TelemetryTable.jsx';
import AskGptPanel from './AskGptPanel.jsx';
import Container from '@mui/material/Container';
import Box from '@mui/material/Box';

export default function Dashboard({ dotnetHelper }) {
    const [data, setData] = useState({ chartData: [], messages: [] });

    useEffect(() => {
        let mounted = true;

        const fetchTelemetry = async () => {
            const json = await dotnetHelper.invokeMethodAsync('GetTelemetry');
            if (!mounted) return;
            setData(JSON.parse(json));
        };

        fetchTelemetry();
        const id = setInterval(fetchTelemetry, 2000);
        return () => {
            mounted = false;
            clearInterval(id);
        };
    }, [dotnetHelper]);

    return (
        <Container maxWidth="md" sx={{ mt: 4 }}>
            <Box sx={{ mb: 4 }}>
                <LiveChart data={data.chartData} />
            </Box>
            <Box sx={{ mb: 4 }}>
                <AskGptPanel onAsk={q => dotnetHelper.invokeMethodAsync('AskGPT', q)} />
            </Box>
            <TelemetryTable messages={data.messages} />
        </Container>
    );
}