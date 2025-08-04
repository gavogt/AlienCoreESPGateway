import React from 'react';
import {
    TableContainer,
    Table,
    TableHead,
    TableRow,
    TableCell,
    TableBody,
    Paper,
    Typography
} from '@mui/material';

export default function TelemetryTable({ messages }) {
    if (!messages || messages.length === 0) {
        return <Typography><em>Waiting for data…</em></Typography>;
    }

    return (
        <TableContainer component={Paper}>
            <Table size="small" aria-label="telemetry data table">
                <TableHead>
                    <TableRow>
                        <TableCell><strong>Time</strong></TableCell>
                        <TableCell><strong>Scout ID</strong></TableCell>
                        <TableCell><strong>Module</strong></TableCell>
                        <TableCell><strong>Value</strong></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {messages.flatMap((msg, msgIdx) =>
                        msg.Modules.map((mod, modIdx) => (
                            <TableRow key={`${msgIdx}-${modIdx}`}>
                                <TableCell>
                                    {new Date(msg.Timestamp).toLocaleTimeString()}
                                </TableCell>
                                <TableCell>{msg.ScoutId}</TableCell>
                                <TableCell>{mod.Type}</TableCell>
                                <TableCell>{mod.Value}</TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
}