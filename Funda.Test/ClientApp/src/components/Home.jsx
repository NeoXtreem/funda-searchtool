import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);
    this.state = { place: 'Amsterdam', garden: false, makelaars: [], loading: false };
    this.handleErrors = this.handleErrors.bind(this);
    this.getTopMakelaars = this.getTopMakelaars.bind(this);
    this.updatePlace = this.updatePlace.bind(this);
    this.updateGarden = this.updateGarden.bind(this);
  }

  handleErrors(response) {
    if (!response.ok) {
      if (response.status === 400) {
        this.setState({
          message: 'An error occurred. Please contact support for assistance.'
        });
      }
    } else {
      this.setState({ message: '' });
    }

    this.setState({
      orderStatus: response.status
    });

    return response;
  }

  updatePlace(e) {
    this.setState({ place: e.target.value });
  }

  updateGarden() {
    this.setState({ garden: !this.state.garden });
  }

  static renderMakelaarsTable(makelaars) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Name</th>
            <th>Properties Listed</th>
          </tr>
        </thead>
        <tbody>
          {makelaars.map(makelaar =>
            <tr key={makelaar.name}>
              <td>{makelaar.name}</td>
              <td>{makelaar.count}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    const contents = this.state.loading ? <p><em>Loading...</em></p> : Home.renderMakelaarsTable(this.state.makelaars);
    const showTopMakelaars = 'Show Top Makelaars';

    return (
      <div>
        <p>
          <span style={{ paddingRight: 10 }}><strong>Place:</strong></span><input type='text' placeholder='Amsterdam' value={this.state.place} onChange={this.updatePlace} />
          <span style={{ paddingLeft: 10, paddingRight: 10 }}><strong>Garden:</strong></span><input type='checkbox' checked={this.state.garden} onChange={this.updateGarden} />
        </p>
        <p><button className='btn btn-primary' onClick={this.getTopMakelaars}>{showTopMakelaars}</button></p>
        <p style={{ color: 'red' }}>{this.state.message}</p>
        <h1 id="tabelLabel">Top Makelaars</h1>
        {contents}
      </div>
    );
  }

  async getTopMakelaars() {
    this.setState({ makelaars: [], loading: true });
    const response = await fetch(`makelaars/gettopten/${this.state.place}/${this.state.garden}`).then(this.handleErrors);
    this.setState({ loading: false });

    if (this.state.message === '') {
      const data = await response.json();
      this.setState({ makelaars: data });
    }
  }
}
